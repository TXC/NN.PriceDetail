using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSV = CsvHelper.Configuration.Attributes;

namespace Web.Entities
{
    [Index(nameof(SKU)), CSV.Delimiter("\t"), CSV.CultureInfo("InvariantCulture"), CSV.HasHeaderRecord]
    public class Price : ICloneable
    {
        [CSV.Name("PriceValueId"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Skapad")]
        public DateTime Created { get; set; }

        [Display(Name = "Ändrad")]
        public DateTime Modified { get; set; }

        [CSV.Name("CatalogEntryCode")]
        public string SKU { get; set; }

        [Display(Name = "Marknad")]
        public string MarketId { get; set; }

        [Display(Name = "Valuta")]
        public string CurrencyCode { get; set; }

        [Display(Name = "Start")]
        public DateTime ValidFrom { get; set; }

#nullable enable
        [CSV.TypeConverter(typeof(CSVConverter.ToDateTime))]
        [Display(Name = "Slut")]
        public DateTime? ValidUntil { get; set; }
#nullable disable

        [Precision(18, 9)]
        [Display(Name = "Pris")]
        public decimal UnitPrice { get; set; }

        [NotMapped, Display(Name = "Start och slut")]
        public DateTimeRange ValidPeriod
        {
            get
            {
                return new DateTimeRange { Start = ValidFrom, End = ValidUntil };
            }
        }

        public object Clone()
        {
            return new Price()
            {
                Id = Id,
                Created = Created,
                Modified = Modified,
                SKU = SKU,
                MarketId = MarketId,
                CurrencyCode = CurrencyCode,
                ValidFrom = ValidFrom,
                ValidUntil = ValidUntil,
            };
        }

        public override string ToString()
            => $"SKU: {SKU}, MarketId: {MarketId}, CurrencyCode: {CurrencyCode}, ValidFrom: {ValidFrom}, ValidUntil: {ValidUntil}, UnitPrice: {UnitPrice}";
    }
}
