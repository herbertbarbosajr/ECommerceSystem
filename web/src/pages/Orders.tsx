import React, { useState, useEffect } from 'react';
import { orders as ordersApi } from 'services/api';
import { toast } from 'react-toastify';
import { Order } from 'types';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Package, Calendar } from 'lucide-react';

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
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">Carregando pedidos...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex items-center gap-2 mb-6">
        <Package className="h-8 w-8" />
        <h1 className="text-3xl font-bold">Gerenciar Pedidos</h1>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Todos os Pedidos</CardTitle>
          <CardDescription>
            Gerencie o status de todos os pedidos do sistema
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID do Pedido</TableHead>
                <TableHead>Data</TableHead>
                <TableHead>Total</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {orders.map((order) => (
                <TableRow key={order.id}>
                  <TableCell className="font-medium">#{order.id}</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      <Calendar className="h-4 w-4" />
                      {new Date(order.createdAt).toLocaleDateString()}
                    </div>
                  </TableCell>
                  <TableCell>R$ {order.totalAmount.toFixed(2)}</TableCell>
                  <TableCell>
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                      order.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                      order.status === 'Confirmed' ? 'bg-blue-100 text-blue-800' :
                      order.status === 'Shipped' ? 'bg-purple-100 text-purple-800' :
                      order.status === 'Delivered' ? 'bg-green-100 text-green-800' :
                      'bg-red-100 text-red-800'
                    }`}>
                      {order.status === 'Pending' ? 'Pendente' :
                       order.status === 'Confirmed' ? 'Confirmado' :
                       order.status === 'Shipped' ? 'Enviado' :
                       order.status === 'Delivered' ? 'Entregue' :
                       'Cancelado'}
                    </span>
                  </TableCell>
                  <TableCell>
                    <Select
                      value={order.status}
                      onValueChange={(value: string) => handleStatusChange(order.id, value)}
                    >
                      <SelectTrigger className="w-32">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="Pending">Pendente</SelectItem>
                        <SelectItem value="Confirmed">Confirmado</SelectItem>
                        <SelectItem value="Shipped">Enviado</SelectItem>
                        <SelectItem value="Delivered">Entregue</SelectItem>
                        <SelectItem value="Cancelled">Cancelado</SelectItem>
                      </SelectContent>
                    </Select>
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

export default AdminOrders;