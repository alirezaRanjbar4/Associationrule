using Hamekaraa.Model.Entities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Hamekaraa.Model.ViewModel.Order
{
    public class AprioriResultVM
    {
        public string? rule { get; set; }

        public double Confidence { get; set; }
    }

    public class OrderAssociationRuleVM
    {
        //public DateTimeOffset? FinalRegistertDateTime { get; set; }

        public string? ServiceName { get; set; }

        public string? Address { get; set; }

        [Display(Name = "وضعیت سرویس")]
        public string? OrderStatus { get; set; }

        [Display(Name = "وضعیت ارسال به متخصص")]
        public string? SendToExpertStatus { get; set; }

        //[Display(Name = "امتیاز مشتری به متخصص")]
        //public float? Rate { get; set; }

    }
}
