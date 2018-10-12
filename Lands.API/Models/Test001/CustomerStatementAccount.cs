namespace Lands.API.Models.Test001
{
    using System;

    public class CustomerStatementAccount
    {
        public string CompanyId { get; set; }

        public string CompanyRif { get; set; }

        public string CompanyDescription { get; set; }

        public string DocumentType { get; set; }

        public int DocumentId { get; set; }

        public DateTime DocumentEmissionDate { get; set; }

        public DateTime DocumentExpirationDate { get; set; }

        public int DocumentDaysToCome { get; set; }

        public int DocumentExpiredDate { get; set; }

        public string DocumentDescription { get; set; }

        public string SellerId { get; set; }

        public string SellerDescription { get; set; }

        public decimal NetAmount { get; set; }

        public decimal Balance { get; set; }

        public string CustomerId { get; set; }

        public string CustomerDescription { get; set; }

        public string ContactPerson { get; set; }

        public string CustomerEmail { get; set; }

        public bool IsTaxPayer { get; set; }
    }
}