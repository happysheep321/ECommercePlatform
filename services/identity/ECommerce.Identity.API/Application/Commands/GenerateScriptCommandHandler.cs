using MediatR;
using ECommerce.BuildingBolcks.EFCore;

namespace ECommerce.Identity.API.Application.Commands
{
    public class GenerateScriptCommandHandler : IRequestHandler<GenerateScriptCommand>
    {
        private readonly IMigrationService migrationService;

        public GenerateScriptCommandHandler(IMigrationService migrationService)
        {
            this.migrationService = migrationService;
        }

        public async Task Handle(GenerateScriptCommand request, CancellationToken cancellationToken)
        {
            var projectPath = Directory.GetCurrentDirectory();
            await migrationService.GenerateScriptAsync(projectPath, null, request.OutputPath);
        }
    }
}
