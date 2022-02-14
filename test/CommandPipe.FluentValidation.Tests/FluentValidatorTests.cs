using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommandPipe.Validations;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace CommandPipe.FluentValidation.Tests
{
    public class FluentValidatorTests
    {
        [Fact]
        public async Task Validator_Should_ReturnInvalid_When_ObjectIsInvalid()
        {
            var provider = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddValidator<TenantInput, UserInputValidator>();
                })
                .BuildServiceProvider();

            var validator = provider.GetService<ICommandValidator<TenantInput>>();

            var result = await validator.ValidateAsync(new TenantInput());

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Validator_Should_TheCorrectNumberOfFails_When_ObjectIsInvalid()
        {
            var provider = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddValidator<TenantInput, UserInputValidator>();
                })
                .BuildServiceProvider();

            var validator = provider.GetService<ICommandValidator<TenantInput>>();

            var result = await validator.ValidateAsync(new TenantInput());

            Assert.Equal(6, result.Errors.Count);
        }

        [Fact]
        public async Task Validator_Should_ReturnInvalid_When_NestedObjectsAreInvalids()
        {
            var provider = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddValidator<TenantInput, UserInputValidator>();
                })
                .BuildServiceProvider();

            var validator = provider.GetService<ICommandValidator<TenantInput>>();

            var tenant = new TenantInput
            {
                TenantId = Guid.NewGuid(),
                Name = "Tenant",
                TenantName = "Tenant",
                CPF = "380.881.290-79",
                FluigIntegration = new FluigIntegration(),
                Products = new List<Product>
                {
                    new Product { ProductKey = "Product-1" },
                    new Product { ProductKey = "Product-2" },
                    new Product { ProductKey = "Product-3" },
                    new Product { ProductKey = "Product-4" }
                },
                Organizations = new List<Organization>
                {
                    new Organization { },
                    new Organization { }
                }
            };

            var result = await validator.ValidateAsync(tenant);

            Assert.Equal(4, result.Errors.Count);
            Assert.Equal("Organization must have Name", result.Errors[0].ErrorMessage);
            Assert.Equal("Organization must have a CNPJ", result.Errors[1].ErrorMessage);
            Assert.Equal("Organization must have Name", result.Errors[2].ErrorMessage);
            Assert.Equal("Organization must have a CNPJ", result.Errors[3].ErrorMessage);
        }

        [Fact]
        public async Task Validator_Should_ReturnInvalid_When_ObjectIsNull()
        {
            var provider = new ServiceCollection()
                .AddCommandPipe(commands =>
                {
                    commands.AddValidator<TenantInput>(validator =>
                    {
                    });
                })
                .BuildServiceProvider();

            var validator = provider.GetService<ICommandValidator<TenantInput>>();

            var result = await validator.ValidateAsync(null);

            Assert.False(result.IsValid);
            var error = Assert.Single(result.Errors);
            Assert.Equal("this", error.PropertyName);
            Assert.Equal("CommandCannotBeNull", error.ErrorCode);
            Assert.Equal("command cannot be null", error.ErrorMessage);
        }

        class UserInputValidator : CommandFluentValidator<TenantInput>
        {
            public override void Configure()
            {
                RuleFor(p => p.TenantId)
                    .NotNull()
                    .WithMessage("TenantId must have a value");

                RuleFor(p => p.Name)
                    .NotEmpty()
                    .WithMessage("Name cannot be empty");

                RuleFor(p => p.TenantName)
                    .NotEmpty()
                    .WithMessage("Tenant must have a value");

                RuleFor(p => p)
                    .Must(p => !string.IsNullOrWhiteSpace(p.CNPJ) || !string.IsNullOrWhiteSpace(p.CPF))
                    .WithMessage("Tenant must have CNPJ or CPF");

                RuleFor(p => p.Products)
                    .NotEmpty()
                    .WithMessage("Tenant must have Products");

                RuleFor(p => p.FluigIntegration)
                    .NotNull()
                    .WithMessage("Tenant must have FluigIntegration");

                RuleForEach(p => p.Products)
                    .SetValidator(new ProductValidator())
                    .When(p => p.Products?.Any() ?? false);

                RuleForEach(p => p.Organizations)
                    .SetValidator(new OrganizationValidator())
                    .When(p => p.Organizations?.Any() ?? false);
            }

            class ProductValidator : CommandFluentValidator<Product>
            {
                public override void Configure()
                {
                    RuleFor(o => o.ProductKey)
                        .NotEmpty();
                }
            }

            class OrganizationValidator : CommandFluentValidator<Organization>
            {
                public override void Configure()
                {
                    RuleFor(o => o.Name)
                        .NotEmpty()
                        .WithMessage("Organization must have Name");

                    RuleFor(o => o.CNPJ)
                        .NotEmpty()
                        .WithMessage("Organization must have a CNPJ");
                }
            }
        }

        public static class Localization
        {
            public const string Errors = "errors";
        }

        class TenantInput
        {
            public Guid? TenantId { get; set; }
            public string Name { get; set; }
            public string TenantName { get; set; }
            public string CPF { get; set; }
            public string CNPJ { get; set; }
            public FluigIntegration FluigIntegration { get; set; }
            public List<Product> Products { get; set; } = new List<Product>();
            public List<Organization> Organizations { get; set; }
        }

        public class FluigIntegration
        {
            public string TenantId { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public bool IsEnable { get; set; }
        }

        public class Product
        {
            public string ProductKey { get; set; }
            public bool GenerateSecret { get; set; }
        }

        public class Organization
        {
            public string Name { get; set; }
            public string CNPJ { get; set; }
        }
    }
}
