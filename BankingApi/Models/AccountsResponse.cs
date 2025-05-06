namespace BankingApi.Models
{
    public class AccountsData
    {
        public List<Account> accounts { get; set; }
    }
    public class AccountsResponse
    {
        public AccountsData data { get; set; }
    }
}
