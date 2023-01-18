using CasaDoCodigo.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace CasaDoCodigo.Repositories
{
    public interface IPedidoRepository
    {
        void AddItem(string codigo);
        Pedido GetPedido();
    }
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {

        private readonly IHttpContextAccessor contextAccessor;
        public PedidoRepository(ApplicationContext context, IHttpContextAccessor contextAccessor) : base(context)
        {
            this.contextAccessor = contextAccessor;
        }

        public void AddItem(string codigo)
        {
            var produto = context.Set<Produto>()
                .Where(p => p.Codigo == codigo).FirstOrDefault();
            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado");
            }
            var pedido = GetPedido();
            var itemPedido = context.Set<ItemPedido>()
                .Where(i => i.Produto.Codigo == codigo && i.Pedido.Id == pedido.Id).SingleOrDefault();

            if(itemPedido == null)
            {
                itemPedido = new ItemPedido(pedido, produto, 1, produto.Preco);
                context.Set<ItemPedido>().Add(itemPedido);
                context.SaveChanges();
            }

        }

        public Pedido GetPedido()
        {
            var pedidoId = GetPedidoId();
            var pedido = dbSet.Where(p => p.Id == pedidoId).SingleOrDefault();

            if(pedido == null)
            {
                pedido = new Pedido();
                dbSet.Add(pedido);
                context.SaveChanges();
                SetPedidoId(pedido.Id);
            }
            return pedido;

        }

        private int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("pedidoId");            
        }

        private void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }
    }
}
