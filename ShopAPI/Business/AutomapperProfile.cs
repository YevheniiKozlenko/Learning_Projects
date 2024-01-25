using AutoMapper;
using Business.Models;
using Data.Entities;
using System.Linq;

namespace Business
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Receipt, ReceiptModel>()
                .ForMember(rm => rm.ReceiptDetailsIds, r => r.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
                .ReverseMap()
                .ForMember(r => r.Customer, opt => opt.Ignore())
                .ForMember(r => r.ReceiptDetails, opt => opt.Ignore());

            CreateMap<Product, ProductModel>()
                .ForMember(pm => pm.CategoryName, p => p.MapFrom(x => x.Category.CategoryName))
                .ForMember(pm => pm.ReceiptDetailIds, p => p.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
                .ReverseMap() 
                .ForMember(p => p.Category, opt => opt.Ignore())
                .ForMember(p => p.ReceiptDetails, opt => opt.Ignore());

            CreateMap<ReceiptDetail, ReceiptDetailModel>()
                .ReverseMap()
                .ForMember(r => r.Product, opt => opt.Ignore())
                .ForMember(r => r.Receipt, opt => opt.Ignore());

            CreateMap<Person, CustomerModel>()
                .ReverseMap()
                .ForMember(p => p.Customer, opt => opt.Ignore());

            CreateMap<Customer, CustomerModel>()
                .ForMember(cm => cm.ReceiptsIds, c => c.MapFrom(x => x.Receipts.Select(r => r.Id)))
                .ReverseMap()
                .ForMember(p => p.Person, opt => opt.Ignore())
                .ForMember(p => p.Receipts, opt => opt.Ignore());

            CreateMap<ProductCategory, ProductCategoryModel>()
                .ForMember(pcm => pcm.ProductIds, pc => pc.MapFrom(x => x.Products.Select(p => p.Id)))
                .ReverseMap()
                .ForMember(pc => pc.Products, opt => opt.Ignore());
        }
    }
}