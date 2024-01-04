using Cotur.DataMining.Association;
using Hamekaraa.Data.Context;
using Hamekaraa.Model.Entities.Enum;
using Hamekaraa.Model.Entities.OrderWrapper;
using Hamekaraa.Model.ViewModel.Order;
using Hamekaraa.Service.Services;
using Hamekaraa.Service.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hamekaraa.web.Controllers
{
    public class AI_TestController : Controller
    {
        private readonly IAIService _service;
        private readonly IAprioriService<OrderAssociationRuleVM> _orderApriori;
        private ApplicationDbContext _context;

        public AI_TestController(IAIService Service, IAprioriService<OrderAssociationRuleVM> orderApriori, ApplicationDbContext context)
        {
            _service = Service;
            _orderApriori = orderApriori;
            _context = context;
        }

        [Route("ai")]
        public IActionResult Index(int support = 20, double confidance = 0.8)
        {
            ViewBag.support = support;
            ViewBag.confidance = confidance;
            List<OrderAssociationRuleVM> success = _context.Orders.AsNoTracking().Where(x => x.InitOrderLevel == OrderInitLevelEnum.Registered).
                Where(x => x.OrderStatus == OrderStatusEnum.Compelet || x.OrderStatus == OrderStatusEnum.DoneByExpert).
                Include(x => x.Service).Include(x => x.Address).ThenInclude(x => x.City).
                Select(x => new OrderAssociationRuleVM()
                {
                    Address = x.Address.City.Name,
                    OrderStatus = x.OrderStatus.GetEnumName(),
                    SendToExpertStatus = x.SendToExpertStatus.GetEnumName(),
                    ServiceName = x.Service.Title,
                }).ToList();

            List<OrderAssociationRuleVM> faileds = _context.Orders.AsNoTracking().Where(x => x.InitOrderLevel == OrderInitLevelEnum.Registered).
                Where(x => x.OrderStatus == OrderStatusEnum.CanceledByAdmin || x.OrderStatus == OrderStatusEnum.CanceledByExpertNoFines
                || x.OrderStatus == OrderStatusEnum.CanceledByExpertWithFines || x.OrderStatus == OrderStatusEnum.CanceledByUserNoFines).
                Include(x => x.Service).Include(x => x.Address).ThenInclude(x => x.City).
                Select(x => new OrderAssociationRuleVM()
                {
                    Address = x.Address.City.Name,
                    OrderStatus = x.OrderStatus.GetEnumName(),
                    SendToExpertStatus = x.SendToExpertStatus.GetEnumName(),
                    ServiceName = x.Service.Title,
                }).ToList();

            var successAssociationRulesModel = _orderApriori.GetAprioriModel(success);
            var failedAssociationRulesModel = _orderApriori.GetAprioriModel(faileds);

            List<AprioriResultVM> successAssociationRules = _service.MineAssociationRules(successAssociationRulesModel, support, confidance);
            List<AprioriResultVM> faildAssociationRules = _service.MineAssociationRules(failedAssociationRulesModel, support, confidance);


            return View(Tuple.Create(successAssociationRules, faildAssociationRules));
        }
    }
}
