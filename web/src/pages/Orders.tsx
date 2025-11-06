import React, { useState, useEffect } from 'react';
import { orders as ordersApi } from '../../services/api';
import { toast } from 'react-toastify';
import { Order } from '../../types';

const AdminOrders: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchOrders = async () => {
    try {
      const data = await ordersApi.getAll(); // Assumindo que o admin getAll busca todos
      setOrders(data);
    } catch (error) {
      toast.error('Falha ao carregar pedidos.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const handleStatusChange = async (id: string, status: string) => {
    try {
      await ordersApi.updateStatus(id, status);
      toast.success('Status do pedido atualizado!');
      fetchOrders(); // Recarrega
    } catch (error) {
      toast.error('Falha ao atualizar status.');
    }
  };

  if (loading) {
    return <div className="text-center mt-8">Carregando pedidos...</div>;
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Gerenciar Pedidos</h1>
      <div className="space-y-4">
        {orders.map((order) => (
          <div key={order.id} className="bg-white p-4 rounded-lg shadow-md">
            <p className="font-semibold">Pedido #{order.id} - Total: R$ {order.total.toFixed(2)}</p>
            <div className="flex items-center gap-4 mt-2">
              <span>Status: {order.status}</span>
              <select 
                onChange={(e) => handleStatusChange(order.id, e.target.value)} 
                defaultValue={order.status}
                className="p-1 border rounded"
              >
                <option value="PENDING">Pendente</option>
                <option value="PAID">Pago</option>
                <option value="SHIPPED">Enviado</option>
                <option value="DELIVERED">Entregue</option>
              </select>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default AdminOrders;