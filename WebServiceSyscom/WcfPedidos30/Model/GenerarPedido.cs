using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfPedidos30.Model
{
    public class GenerarPedido
    {
        public Log GenerarPedidos(DtPedido pedido, out List<PedidoResponse> datPedido)
        {
            datPedido = new List<PedidoResponse>();
            List<PedidoResponse> list = new List<PedidoResponse>();
            Log _error = new Log { Codigo = "error", Descripcion = "error" };
            return _error;
        }
    }
}