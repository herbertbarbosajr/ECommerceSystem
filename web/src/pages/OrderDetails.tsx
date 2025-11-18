import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { orders as ordersApi } from 'services/api';
import { toast } from 'react-toastify';
import { Order } from 'types';

const OrderDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchOrder = async () => {
      if (!id) return;
      try {
        const data = await ordersApi.getById(id);
        setOrder(data);
      } catch (error) {
        toast.error('Falha ao carregar detalhes do pedido.');
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchOrder();
  }, [id]);

  if (loading) {
    return <div className="text-center mt-8">Carregando...</div>;
  }

  if (!order) {
    return <div className="text-center mt-8 text-red-500">Pedido não encontrado.</div>;
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-2">Detalhes do Pedido #{order.id}</h1>
      <p className="text-gray-500 mb-6">Data: {new Date(order.createdAt).toLocaleDateString()}</p>
      
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h2 className="text-2xl font-semibold mb-4">Itens</h2>
        {order.orderItems.map(item => (
          <div key={item.id} className="flex justify-between items-center border-b py-2">
            <p>{item.product.name} (x{item.quantity})</p>
            <p>R$ {item.totalPrice.toFixed(2)}</p>
          </div>
        ))}
        <div className="text-right mt-4 text-xl font-bold">Total: R$ {order.totalAmount.toFixed(2)}</div>
      </div>
    </div>
  );
};

export default OrderDetails;