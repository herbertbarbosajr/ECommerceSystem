import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { products as productsApi } from '../services/api';
import { toast } from 'react-toastify';
import { Product } from '../types';

// TODO: Implementar um contexto ou estado global para o carrinho
const addToCart = (product: Product, quantity: number) => {
  console.log(`Adicionado ${quantity} de ${product.name} ao carrinho.`);
  toast.success(`${product.name} adicionado ao carrinho!`);
  // Aqui você adicionaria a lógica para o carrinho de compras
};

const ProductDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    const fetchProduct = async () => {
      if (!id) return;
      try {
        const data = await productsApi.getById(id);
        setProduct(data);
      } catch (error) {
        toast.error('Falha ao carregar detalhes do produto.');
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id]);

  if (loading) {
    return <div className="text-center mt-8">Carregando...</div>;
  }

  if (!product) {
    return <div className="text-center mt-8 text-red-500">Produto não encontrado.</div>;
  }

  const handleAddToCart = () => {
    addToCart(product, quantity);
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* <img src={product.imageUrl || '/placeholder.png'} alt={product.name} className="w-full rounded-lg shadow-md" /> */}
        <div>
          <h1 className="text-4xl font-bold mb-2">{product.name}</h1>
          <p className="text-2xl text-gray-700 mb-4">R$ {product.price.toFixed(2)}</p>
          <p className="text-gray-600 mb-6">{product.description || 'Sem descrição disponível.'}</p>
          <div className="flex items-center gap-4">
            <button onClick={handleAddToCart} className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700">Adicionar ao Carrinho</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetails;