using FluentValidation;
using CompetitionsTracking.Application.DTOs.Score;

namespace CompetitionsTracking.Application.Validators.Score
{
    public class ScoreRequestDtoValidator : AbstractValidator<ScoreRequestDto>
    {
        public ScoreRequestDtoValidator()
        {
            RuleFor(x => x.EntryId).GreaterThan(0).WithMessage("EntryId must be greater than 0.");
            RuleFor(x => x.JudgeId).GreaterThan(0).WithMessage("JudgeId must be greater than 0.");
            RuleFor(x => x.ScoreValue).InclusiveBetween(0, 30).WithMessage("Score value must be between 0 and 30.");

            // Constraints for Artistry (A)
            RuleFor(x => x.ScoreValue)
                .LessThanOrEqualTo(10.0f)
                .When(x => x.Type == ScoreType.A)
                .WithMessage("Artistry score cannot exceed 10.0.");

            // Constraints for Execution (E)
            RuleFor(x => x.ScoreValue)
                .LessThanOrEqualTo(10.0f)
                .When(x => x.Type == ScoreType.E)
                .WithMessage("Execution score cannot exceed 10.0.");

            // Constraints for Difficulty Apparatus (DA)
            RuleFor(x => x.ScoreValue)
                .LessThanOrEqualTo(15.0f)
                .When(x => x.Type == ScoreType.DA)
                .WithMessage("Apparatus Difficulty (DA) cannot exceed 15.0.");

            // Constraints for Difficulty Body (DB)
            RuleSet("DB_Breakdown", () =>
            {
                RuleFor(x => x.ElementCount)
                    .NotNull()
                    .InclusiveBetween(1, 8)
                    .When(x => x.Type == ScoreType.DB)
                    .WithMessage("DB must have between 1 and 8 counted elements.");

                RuleFor(x => x.DynamicRotationCount)
                    .NotNull()
                    .LessThanOrEqualTo(4)
                    .When(x => x.Type == ScoreType.DB)
                    .WithMessage("DB can have a maximum of 4 dynamic rotation (R) elements.");

                RuleFor(x => x.JumpCount)
                    .NotNull()
                    .GreaterThanOrEqualTo(1)
                    .When(x => x.Type == ScoreType.DB)
                    .WithMessage("DB must include at least 1 Jump.");

                RuleFor(x => x.BalanceCount)
                    .NotNull()
                    .GreaterThanOrEqualTo(1)
                    .When(x => x.Type == ScoreType.DB)
                    .WithMessage("DB must include at least 1 Balance.");

                RuleFor(x => x.RotationCount)
                    .NotNull()
                    .GreaterThanOrEqualTo(1)
                    .When(x => x.Type == ScoreType.DB)
                    .WithMessage("DB must include at least 1 Rotation.");
            });
        }
    }

}
