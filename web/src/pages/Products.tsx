import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { products as productsApi } from 'services/api';
import { toast } from 'react-toastify';
import { Product } from 'types';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Trash2, Plus } from 'lucide-react';

type ProductFormData = Omit<Product, 'id'>;

const AdminProducts: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const form = useForm<ProductFormData>();

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
      // A API espera números para preço e quantidade
      const productData = {
        ...data,
        price: Number(data.price),
        stockQuantity: Number(data.stockQuantity)
      };
      await productsApi.create(productData);
      toast.success('Produto criado com sucesso!');
      form.reset();
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

      <Card className="mb-8">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Plus className="h-5 w-5" />
            Adicionar Novo Produto
          </CardTitle>
          <CardDescription>
            Preencha os dados do produto abaixo
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <FormField
                  control={form.control}
                  name="name"
                  rules={{ required: 'Nome é obrigatório' }}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Nome do Produto</FormLabel>
                      <FormControl>
                        <Input {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="price"
                  rules={{ required: 'Preço é obrigatório' }}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Preço</FormLabel>
                      <FormControl>
                        <Input type="number" step="0.01" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="stockQuantity"
                  rules={{ required: 'Quantidade é obrigatória' }}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Quantidade em Estoque</FormLabel>
                      <FormControl>
                        <Input type="number" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="description"
                  render={({ field }) => (
                    <FormItem className="md:col-span-2 lg:col-span-1">
                      <FormLabel>Descrição</FormLabel>
                      <FormControl>
                        <Input {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="imageUrl"
                  render={({ field }) => (
                    <FormItem className="md:col-span-2">
                      <FormLabel>URL da Imagem</FormLabel>
                      <FormControl>
                        <Input {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <Button type="submit">
                <Plus className="h-4 w-4 mr-2" />
                Adicionar Produto
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Produtos Cadastrados</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nome</TableHead>
                <TableHead>Preço</TableHead>
                <TableHead>Estoque</TableHead>
                <TableHead>Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {products.map((product) => (
                <TableRow key={product.id}>
                  <TableCell className="font-medium">{product.name}</TableCell>
                  <TableCell>R$ {product.price.toFixed(2)}</TableCell>
                  <TableCell>{product.stockQuantity}</TableCell>
                  <TableCell>
                    <Button
                      variant="destructive"
                      size="sm"
                      onClick={() => handleDelete(product.id)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );
};

export default AdminProducts;