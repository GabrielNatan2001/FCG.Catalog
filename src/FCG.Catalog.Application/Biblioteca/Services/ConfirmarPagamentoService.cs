using FCG.Catalog.Application.Messaging.Events;
using FCG.Catalog.Domain.Biblioteca.Entities;
using FCG.Catalog.Domain.Biblioteca.Interfaces;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;
using FCG.Catalog.Domain.Jogo.Interfaces;
using FCG.Catalog.Domain.Pedidos.Interfaces;

namespace FCG.Catalog.Application.Biblioteca.Services;

public class ConfirmarPagamentoService
{
    private const string PaymentApproved = "Approved";

    private readonly IPedidoRepository _pedidoRepository;
    private readonly IJogoRepository _jogoRepository;
    private readonly IBibliotecaRepository _bibliotecaRepository;

    public ConfirmarPagamentoService(
        IPedidoRepository pedidoRepository,
        IJogoRepository jogoRepository,
        IBibliotecaRepository bibliotecaRepository)
    {
        _pedidoRepository = pedidoRepository;
        _jogoRepository = jogoRepository;
        _bibliotecaRepository = bibliotecaRepository;
    }

    public async Task Execute(PaymentProcessedEvent payment)
    {
        var pedido = await _pedidoRepository.ObterPorId(payment.OrderId);
        if (pedido is null)
            throw new DomainException("Pedido não encontrado.");

        if (pedido.Status != EPedidoStatus.Pending)
            return;

        if (string.Equals(payment.Status, PaymentApproved, StringComparison.OrdinalIgnoreCase))
        {
            var jogo = await _jogoRepository.ObterPorId(payment.GameId);
            if (jogo is null)
                throw new DomainException("Jogo não encontrado.");

            var biblioteca = await _bibliotecaRepository.ObterPorUsuarioId(payment.UserId);
            if (biblioteca is null)
            {
                biblioteca = BibliotecaEntity.Criar(payment.UserId);
                biblioteca.AdicionarJogo(payment.GameId);
                await _bibliotecaRepository.Adicionar(biblioteca);
            }
            else if (!biblioteca.PossuiJogo(payment.GameId))
            {
                biblioteca.AdicionarJogo(payment.GameId);
            }

            pedido.Completar();
        }
        else
        {
            pedido.Rejeitar();
        }

        await _pedidoRepository.SalvarAlteracoes();
    }
}
