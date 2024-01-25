using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Business.Validation;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private IUnitOfWork UnitOfWork { get; }

        private IMapper Mapper { get; }

        private readonly DateTime _validBirthDateMinValue = DateTime.Today.AddYears(-120); 

        private readonly DateTime _validBirthDateMaxValue = DateTime.Today.AddYears(-18);

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            IEnumerable<Customer> customerDtos = 
                await UnitOfWork.CustomerRepository.GetAllWithDetailsAsync();

            if (!customerDtos.Any())
                throw new MarketException($"Customers list is empty");

            IEnumerable<CustomerModel> customerModels = customerDtos
                .Where(c => c.Receipts
                    .Any(r => r.ReceiptDetails
                        .Any(rd => rd.ProductId == productId)))
                .Select(x =>
                {
                    CustomerModel customerModel = Mapper.Map<CustomerModel>(x);
                    Mapper.Map(x.Person, customerModel);
                    return customerModel;
                });

            if (!customerModels.Any())
                throw new MarketException($"Customers not found by product id");

            return customerModels;
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            IEnumerable<Customer> customerDtos = 
                await UnitOfWork.CustomerRepository.GetAllWithDetailsAsync();

            if (!customerDtos.Any())
                throw new MarketException($"Customers list is empty");

            IEnumerable<CustomerModel> customerModels = customerDtos
                .Select(x =>
                {
                    CustomerModel customerModel = Mapper.Map<CustomerModel>(x);
                    Mapper.Map(x.Person, customerModel);
                    return customerModel;
                });

            return customerModels;
        }

        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            Customer customerDto = 
                await UnitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);

            if (customerDto is null)
                throw new MarketException($"Customer with id={id} not found");

            CustomerModel customerModel = Mapper.Map<CustomerModel>(customerDto);
            Mapper.Map(customerDto.Person, customerModel);
            return customerModel;
        }

        public async Task AddAsync(CustomerModel model)
        {
            Validate(model);
            Customer customerDto = Mapper.Map<Customer>(model);
            Person personDto = Mapper.Map<Person>(model);
            customerDto.Person = personDto;
            personDto.Customer = customerDto;
            await UnitOfWork.CustomerRepository.AddAsync(customerDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            Validate(model);
            Customer customerDto = Mapper.Map<Customer>(model);
            Person personDto = Mapper.Map<Person>(model);
            customerDto.Person = personDto;
            personDto.Customer = customerDto;
            UnitOfWork.CustomerRepository.Update(customerDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await UnitOfWork.CustomerRepository.DeleteByIdAsync(modelId);
            await UnitOfWork.SaveAsync();
        }

        private void Validate(CustomerModel model)
        {
            if (model is null)
                throw new MarketException($"Given customer model is null");
            if (String.IsNullOrEmpty(model.Name))
                throw new MarketException($"Given customer model name is null or empty");
            if (String.IsNullOrEmpty(model.Surname))
                throw new MarketException($"Given customer model surname is null or empty");
            if (model.DiscountValue < 0)
                throw new MarketException($"Discount value can't be negative");
            if (model.BirthDate < _validBirthDateMinValue || 
                model.BirthDate > _validBirthDateMaxValue)
                throw new MarketException($"Invalid customer birth date");
        }
    }
}
