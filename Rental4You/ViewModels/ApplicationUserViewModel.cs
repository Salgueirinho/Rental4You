namespace Rental4You.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string UserId { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string UserName { get; set; }
        public bool Ativo { get; set; }
        public List<string> Roles { get; set; }
    }
}
