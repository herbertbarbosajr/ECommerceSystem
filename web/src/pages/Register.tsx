import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { toast } from 'react-toastify';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';

export default function Register() {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.currentTarget.id]: e.currentTarget.value });
  };

  const validatePassword = (password: string): boolean => {
    // At least 8 characters, one uppercase, one lowercase, one number
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d@$!%*?&]{8,}$/;
    return regex.test(password);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (formData.password !== formData.confirmPassword) {
      toast.error('As senhas não coincidem!');
      return;
    }

    if (!validatePassword(formData.password)) {
      toast.error('A senha deve ter pelo menos 8 caracteres, incluindo uma letra maiúscula, uma minúscula e um número.');
      return;
    }

    setLoading(true);
    try {
      await register(formData.firstName, formData.lastName, formData.email, formData.password);
      toast.success('Registro realizado com sucesso! Faça o login.');
      navigate('/login');
    } catch (error: any) {
      const errorMessage = error?.response?.data?.message || error?.message || 'Falha no registro. Tente novamente.';
      toast.error(errorMessage);
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle className="text-2xl text-center">Criar Conta</CardTitle>
          <CardDescription className="text-center">
            Preencha os dados para criar sua conta
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="firstName">Nome</Label>
                <Input
                  id="firstName"
                  type="text"
                  placeholder="João"
                  value={formData.firstName}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="lastName">Sobrenome</Label>
                <Input
                  id="lastName"
                  type="text"
                  placeholder="Silva"
                  value={formData.lastName}
                  onChange={handleChange}
                  required
                />
                </div>
              </div>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="seu@email.com"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Senha</Label>
              <Input
                id="password"
                type="password"
                placeholder="••••••••"
                value={formData.password}
                onChange={handleChange}
                required
              />
              <p className="text-xs text-gray-500">
                Mínimo 8 caracteres, com letra maiúscula, minúscula e número
              </p>
            </div>
            <div className="space-y-2">
              <Label htmlFor="confirmPassword">Confirmar Senha</Label>
              <Input
                id="confirmPassword"
                type="password"
                placeholder="••••••••"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
              />
          </div>
            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? 'Criando conta...' : 'Criar Conta'}
            </Button>
            <div className="text-center text-sm">
              <span className="text-gray-600">Já tem uma conta? </span>
              <Link to="/login" className="text-blue-600 hover:underline">
                Faça login
              </Link>
        </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
