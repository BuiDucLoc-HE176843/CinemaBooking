using CinemaBooking.Data;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CinemaBooking.Services.BackgroundServices
{
    public class SePaySyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        public SePaySyncService(
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _client = httpClientFactory.CreateClient();
            _config = config;

            _client.Timeout = TimeSpan.FromSeconds(10);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Sync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SePay sync error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task Sync(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var lastDate = await db.BankTransactions
                .MaxAsync(x => (DateTime?)x.TransactionDate, token);

            var url = "https://my.sepay.vn/userapi/transactions/list?limit=50";

            if (lastDate.HasValue)
            {
                url += "&transaction_date_min=" +
                       lastDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["SePay:ApiToken"]);

            var response = await _client.SendAsync(request, token);

            if (!response.IsSuccessStatusCode)
                return;

            var stream = await response.Content.ReadAsStreamAsync(token);

            var result = await JsonSerializer.DeserializeAsync<SePayResponse>(
                stream, _jsonOptions, token);

            var transactions = result?.Transactions;

            if (transactions == null || transactions.Count == 0)
                return;

            var filtered = transactions
                .Where(t => t.Sub_Account == "9624750HMH")
                .ToList();

            if (filtered.Count == 0)
                return;

            var ids = filtered.Select(x => x.Id).ToList();

            var existingIds = (await db.BankTransactions
                .Where(x => ids.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(token))
                .ToHashSet();

            var newTransactions = new List<BankTransaction>(filtered.Count);

            foreach (var x in filtered)
            {
                if (existingIds.Contains(x.Id))
                    continue;

                newTransactions.Add(new BankTransaction
                {
                    Id = x.Id,
                    BankBrandName = x.Bank_Brand_Name,
                    AccountNumber = x.Account_Number,
                    TransactionDate = DateTime.Parse(x.Transaction_Date),
                    AmountOut = decimal.Parse(x.Amount_Out, CultureInfo.InvariantCulture),
                    AmountIn = decimal.Parse(x.Amount_In, CultureInfo.InvariantCulture),
                    Accumulated = decimal.Parse(x.Accumulated, CultureInfo.InvariantCulture),
                    TransactionContent = x.Transaction_Content,
                    ReferenceNumber = x.Reference_Number,
                    Code = x.Code,
                    SubAccount = x.Sub_Account,
                    BankAccountId = x.Bank_Account_Id
                });
            }

            if (newTransactions.Count == 0)
                return;

            await db.BankTransactions.AddRangeAsync(newTransactions, token);

            await db.SaveChangesAsync(token);
        }
    }
}
