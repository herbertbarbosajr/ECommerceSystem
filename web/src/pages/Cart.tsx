import React from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { orders as ordersApi } from '../services/api';

// Mock de dados do carrinho. Em uma aplicação real, isso viria de um estado global (Context, Redux, etc.).
const mockCartItems = [
  { productId: 'mock-id-1', name: 'Produto de Exemplo 1', price: 99.90, quantity: 1 },
  { productId: 'mock-id-2', name: 'Produto de Exemplo 2', price: 49.90, quantity: 2 },
];

const Cart: React.FC = () => {
  const navigate = useNavigate();
  const cartItems = mockCartItems; // Usando o mock

  const handleCheckout = async () => {
    if (cartItems.length === 0) {
      toast.warn('Seu carrinho está vazio.');
      return;
    }

    const orderData = {
      items: cartItems.map(item => ({ productId: item.productId, quantity: item.quantity })),
    };

    try {
      await ordersApi.create(orderData);
      toast.success('Pedido realizado com sucesso!');
      // Limpar o carrinho aqui
      navigate('/orders');
    } catch (error) {
      toast.error('Falha ao finalizar o pedido.');
      console.error(error);
    }
  };

  const totalPrice = cartItems.reduce((total, item) => total + item.price * item.quantity, 0);

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Carrinho de Compras</h1>
      {cartItems.length === 0 ? (
        <p>Seu carrinho está vazio.</p>
      ) : (
        <div>
          {/* Listagem de itens aqui */}
          <div className="text-right mt-6">
            <p className="text-2xl font-bold">Total: R$ {totalPrice.toFixed(2)}</p>
            <button onClick={handleCheckout} className="bg-green-600 text-white px-8 py-3 rounded-lg hover:bg-green-700 mt-4">Finalizar Compra</button>
          </div>
        </div>
      )}
    </div>
  );
};

export default Cart;