namespace CinemaBooking.DTOs.Responses
{
    public class SePayResponse
    {
        public int Status { get; set; }
        public List<SePayTransaction> Transactions { get; set; }
    }

    public class SePayTransaction
    {
        public string Id { get; set; }
        public string Bank_Brand_Name { get; set; }
        public string Account_Number { get; set; }
        public string Transaction_Date { get; set; }
        public string Amount_Out { get; set; }
        public string Amount_In { get; set; }
        public string Accumulated { get; set; }
        public string Transaction_Content { get; set; }
        public string Reference_Number { get; set; }
        public string Code { get; set; }
        public string Sub_Account { get; set; }
        public string Bank_Account_Id { get; set; }
    }
}
