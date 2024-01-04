using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hamekaraa.Data.Context;
using Hamekaraa.Model.ViewModel;
using Hamekaraa.Model.ViewModel.Order;
using Hamekaraa.Service.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace Hamekaraa.Service.Services
{
    public interface IAprioriService<T> where T : class
    {
        List<List<string>> GetAprioriModel(List<T> data);
    }

    public class AprioriService<T> : IAprioriService<T> where T : class
    {
        private ApplicationDbContext _context;
        private DbSet<T> table;

        public AprioriService(ApplicationDbContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }




        public List<List<string>> GetAprioriModel(List<T> data)
        {
            var fields = typeof(T).GetProperties();
            return data.Select(data =>
            {
                var values = fields.Select(field => field.GetValue(data)?.ToString()).Where(x => x != null).ToList();
                return values;
            }).ToList();
        }
    }
}
