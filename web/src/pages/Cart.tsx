import React from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { orders as ordersApi } from 'services/api';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { ShoppingCart, CreditCard, Trash2 } from 'lucide-react';

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
      <div className="flex items-center gap-2 mb-6">
        <ShoppingCart className="h-8 w-8" />
        <h1 className="text-3xl font-bold">Carrinho de Compras</h1>
      </div>

      {cartItems.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12">
            <ShoppingCart className="h-16 w-16 text-muted-foreground mb-4" />
            <h3 className="text-lg font-semibold mb-2">Seu carrinho está vazio</h3>
            <p className="text-muted-foreground text-center">
              Adicione alguns produtos ao seu carrinho para continuar comprando.
            </p>
          </CardContent>
        </Card>
      ) : (
        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Itens no Carrinho</CardTitle>
              <CardDescription>
                Revise os produtos antes de finalizar a compra
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Produto</TableHead>
                    <TableHead>Quantidade</TableHead>
                    <TableHead>Preço Unitário</TableHead>
                    <TableHead>Total</TableHead>
                    <TableHead>Ações</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {cartItems.map(item => (
                    <TableRow key={item.productId}>
                      <TableCell className="font-medium">{item.name}</TableCell>
                      <TableCell>{item.quantity}</TableCell>
                      <TableCell>R$ {item.price.toFixed(2)}</TableCell>
                      <TableCell>R$ {(item.price * item.quantity).toFixed(2)}</TableCell>
                      <TableCell>
                        <Button variant="destructive" size="sm">
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>

          <Card>
            <CardContent className="pt-6">
              <div className="flex justify-between items-center">
                <div>
                  <p className="text-sm text-muted-foreground">Total do pedido</p>
                  <p className="text-3xl font-bold">R$ {totalPrice.toFixed(2)}</p>
                </div>
                <Button onClick={handleCheckout} size="lg">
                  <CreditCard className="h-5 w-5 mr-2" />
                  Finalizar Compra
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      )}
    </div>
  );
};

export default Cart;