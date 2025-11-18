import React from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useSearchParams, Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { auth } from 'services/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';

type ResetPasswordFormData = {
  email: string;
  token: string;
  newPassword: string;
  confirmPassword: string;
};

const ResetPassword = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  
  const form = useForm<ResetPasswordFormData>({
    defaultValues: {
      email: searchParams.get('email') || '',
      token: searchParams.get('token') || '',
      newPassword: '',
      confirmPassword: ''
    }
  });

  const onSubmit = async (data: ResetPasswordFormData) => {
    if (data.newPassword !== data.confirmPassword) {
      toast.error('As senhas não coincidem!');
      return;
    }

    try {
      await auth.resetPassword({
        email: data.email,
        token: data.token,
        newPassword: data.newPassword
      });
      toast.success('Senha redefinida com sucesso! Você já pode fazer o login.');
      navigate('/login');
    } catch (error: any) {
      const errorMessage = error?.response?.data?.message || error?.message || 'Falha ao redefinir a senha. O link pode ter expirado.';
      toast.error(errorMessage);
      console.error('Reset password error:', error);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle className="text-2xl text-center">Redefinir Senha</CardTitle>
          <CardDescription className="text-center">
            Digite sua nova senha
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
              <input type="hidden" {...form.register('email')} />
              <input type="hidden" {...form.register('token')} />
              
              <FormField
                control={form.control}
                name="newPassword"
                rules={{ required: 'Nova senha é obrigatória', minLength: { value: 6, message: 'A senha deve ter no mínimo 6 caracteres' } }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Nova Senha</FormLabel>
                    <FormControl>
                      <Input type="password" placeholder="••••••••" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="confirmPassword"
                rules={{ 
                  required: 'Confirmação de senha é obrigatória',
                  validate: (value) => value === form.watch('newPassword') || 'As senhas não coincidem'
                }}
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Confirmar Nova Senha</FormLabel>
                    <FormControl>
                      <Input type="password" placeholder="••••••••" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              <Button type="submit" className="w-full" disabled={form.formState.isSubmitting}>
                {form.formState.isSubmitting ? 'Redefinindo...' : 'Redefinir Senha'}
              </Button>
              
              <div className="text-center text-sm">
                <Link to="/login" className="text-blue-600 hover:underline">
                  Voltar para o login
                </Link>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
};

export default ResetPassword;
