import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { products as productsApi } from 'services/api';
import { toast } from 'react-toastify';
import { Product } from 'types';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { ShoppingBag, Star } from 'lucide-react';

const Home: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const data = await productsApi.getAll(true);
        setProducts(data);
      } catch (error) {
        toast.error('Falha ao carregar produtos.');
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">Carregando produtos...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex items-center justify-center gap-2 mb-8">
        <ShoppingBag className="h-8 w-8" />
        <h1 className="text-3xl font-bold text-center">Nossos Produtos</h1>
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {products && products.length > 0 ? products.map((product) => (
          <Link to={`/products/${product.id}`} key={product.id} className="block">
            <Card className="h-full hover:shadow-lg transition-shadow duration-300">
              <CardHeader>
                <CardTitle className="text-lg">{product.name}</CardTitle>
                <CardDescription className="flex items-center gap-1">
                  <Star className="h-4 w-4 fill-yellow-400 text-yellow-400" />
                  <span>4.5</span>
                </CardDescription>
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-bold text-primary">R$ {product.price.toFixed(2)}</p>
                <p className="text-sm text-muted-foreground mt-2">
                  {product.description || 'Produto de alta qualidade'}
                </p>
              </CardContent>
            </Card>
          </Link>
        )) : (
          <div className="col-span-full text-center py-8 text-gray-500">
            Nenhum produto disponível no momento.
          </div>
        )}
      </div>
    </div>
  );
};

export default Home;