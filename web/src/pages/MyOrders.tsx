import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { orders as ordersApi } from 'services/api';
import { toast } from 'react-toastify';
import { Order } from 'types';

const MyOrders: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        // Assumindo que a API tem um método para buscar os pedidos do usuário logado
        const data = await ordersApi.getMyOrders();
        setOrders(data);
      } catch (error) {
        toast.error('Falha ao carregar seus pedidos.');
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchOrders();
  }, []);

  if (loading) {
    return <div className="text-center mt-8">Carregando seus pedidos...</div>;
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Meus Pedidos</h1>
      <div className="space-y-4">
        {orders.length > 0 ? orders.map((order) => (
          <Link to={`/orders/${order.id}`} key={order.id} className="block bg-white p-4 rounded-lg shadow-md hover:bg-gray-50 transition-colors">
            <p className="font-semibold">Pedido #{order.id} - <span className="font-normal text-gray-600">Data: {new Date(order.createdAt).toLocaleDateString()}</span></p>
            <p>Status: {order.status} - Total: R$ {order.totalAmount.toFixed(2)}</p>
          </Link>
        )) : <p>Você ainda não fez nenhum pedido.</p>}
      </div>
    </div>
  );
};

export default MyOrders;