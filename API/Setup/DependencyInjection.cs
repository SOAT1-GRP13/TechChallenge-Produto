using MediatR;
using Infra.Catalogo;
using Domain.Catalogo;
using Infra.Catalogo.Repository;
using Application.Catalogo.Queries;
using Application.Catalogo.Commands;
using Application.Catalogo.Handlers;
using Application.Catalogo.Boundaries;
using Domain.Base.Communication.Mediator;
using Domain.Base.Messages.CommonMessages.Notifications;

namespace API.Setup
{
    public static class DependencyInjection
    { 
        public static void RegisterServices(this IServiceCollection services)
        {
            //Mediator
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            //Domain Notifications 
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Catalogo
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IProdutosQueries, ProdutosQueries>();
            services.AddScoped<IRequestHandler<AdicionarProdutoCommand, ProdutoOutput>, AdicionarProdutoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarProdutoCommand, ProdutoOutput>, AtualizarProdutoCommandHandler>();
            services.AddScoped<IRequestHandler<RemoverProdutoCommand, bool>, RemoverProdutoCommandHandler>();
            services.AddScoped<CatalogoContext>();
        }
    }
}
