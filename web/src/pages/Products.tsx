import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { products as productsApi } from '../../services/api';
import { toast } from 'react-toastify';
import { Product } from '../../types';

type ProductFormData = Omit<Product, 'id'>;

const AdminProducts: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const { register, handleSubmit, reset } = useForm<ProductFormData>();

  const fetchProducts = async () => {
    try {
      const data = await productsApi.getAll();
      setProducts(data);
    } catch (error) {
      toast.error('Falha ao carregar produtos.');
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const onSubmit = async (data: ProductFormData) => {
    try {
      // A API espera um número para o preço
      const productData = { ...data, price: Number(data.price) };
      await productsApi.create(productData);
      toast.success('Produto criado com sucesso!');
      reset();
      fetchProducts(); // Recarrega a lista
    } catch (error) {
      toast.error('Falha ao criar produto.');
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Tem certeza que deseja excluir este produto?')) {
      try {
        await productsApi.delete(id);
        toast.success('Produto excluído com sucesso!');
        fetchProducts(); // Recarrega a lista
      } catch (error) {
        toast.error('Falha ao excluir produto.');
      }
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Gerenciar Produtos</h1>

      <form onSubmit={handleSubmit(onSubmit)} className="mb-8 p-4 bg-gray-100 rounded-lg">
        <h2 className="text-2xl mb-4">Adicionar Novo Produto</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <input {...register('name', { required: true })} placeholder="Nome do Produto" className="p-2 border rounded" />
          <input type="number" step="0.01" {...register('price', { required: true, valueAsNumber: true })} placeholder="Preço" className="p-2 border rounded" />
          <input {...register('description')} placeholder="Descrição" className="p-2 border rounded" />
        </div>
        <button type="submit" className="mt-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">Adicionar Produto</button>
      </form>

      <div>
        {products.map((product) => (
          <div key={product.id} className="flex justify-between items-center p-2 border-b">
            <span>{product.name} - R$ {product.price.toFixed(2)}</span>
            <div>
              {/* Botão de editar pode ser implementado aqui */}
              <button onClick={() => handleDelete(product.id)} className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600 ml-2">Excluir</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default AdminProducts;