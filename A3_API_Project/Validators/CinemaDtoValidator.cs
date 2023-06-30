using A3_API_Project.Models.Cinema.DTO;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A3_API_Project.Validators
{
    public class CinemaDtoValidator : AbstractValidator<CinemaDTO>
    {
        public CinemaDtoValidator()
        {
            RuleFor(x => x.AmrCidadeRegiaoId).NotEmpty().WithMessage("ID da amarração Cidade/Região deve ser informado.");
            RuleFor(x => x.NomeCinema).NotEmpty().WithMessage("Nome do cinema é obrigatório.");
        }

        public static bool BeAValidPostcode(CinemaDTO cinemaDTO)
        {
            var validator = new CinemaDtoValidator();
            ValidationResult results = validator.Validate(cinemaDTO);

            if (!results.IsValid)
            {

                var ex = new ValidationException("Erro de validação de dados.");
                foreach (var error in results.Errors)
                {
                    ex.Data.Add(error.PropertyName, error.ErrorMessage);
                }

                throw ex;
            }

            return true;
        }
    }
}
