using MediatR;
using ECommerce.BuildingBolcks.EFCore;

namespace ECommerce.Identity.API.Application.Commands
{
    public class CreateMigrationCommandHandler : IRequestHandler<CreateMigrationCommand>
    {
        private readonly IMigrationService migrationService;

        public CreateMigrationCommandHandler(IMigrationService migrationService)
        {
            this.migrationService = migrationService;
        }

        public async Task Handle(CreateMigrationCommand request, CancellationToken cancellationToken)
        {
            var projectPath = Directory.GetCurrentDirectory();
            await migrationService.CreateMigrationAsync(request.MigrationName, projectPath);
        }
    }
}
