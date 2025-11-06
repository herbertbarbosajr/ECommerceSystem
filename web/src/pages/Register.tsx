import React from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { auth } from '../services/api';
import { RegisterRequest } from '../types';

const Register: React.FC = () => {
  const { register, handleSubmit, formState: { errors } } = useForm<RegisterRequest>();
  const navigate = useNavigate();

  const onSubmit = async (data: RegisterRequest) => {
    try {
      await auth.register(data);
      toast.success('Registro realizado com sucesso! Faça o login.');
      navigate('/login');
    } catch (error) {
      toast.error('Falha no registro. Verifique seus dados.');
      console.error(error);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="px-8 py-6 mt-4 text-left bg-white shadow-lg rounded-lg">
        <h3 className="text-2xl font-bold text-center">Crie sua conta</h3>
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="mt-4">
            <div>
              <label className="block" htmlFor="email">Email</label>
              <input
                type="email"
                placeholder="Email"
                {...register('email', { required: 'Email é obrigatório' })}
                className="w-full px-4 py-2 mt-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-600"
              />
              {errors.email && <span className="text-xs text-red-400">{errors.email.message}</span>}
            </div>
            <div className="mt-4">
              <label className="block" htmlFor="password">Senha</label>
              <input
                type="password"
                placeholder="Senha"
                {...register('password', { required: 'Senha é obrigatória' })}
                className="w-full px-4 py-2 mt-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-600"
              />
              {errors.password && <span className="text-xs text-red-400">{errors.password.message}</span>}
            </div>
            <div className="mt-4">
              <label className="block" htmlFor="role">Tipo de Usuário</label>
              <select 
                {...register('role', { required: 'Tipo de usuário é obrigatório' })}
                className="w-full px-4 py-2 mt-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-600"
              >
                <option value="CUSTOMER">Cliente</option>
                <option value="ADMIN">Administrador</option>
              </select>
              {errors.role && <span className="text-xs text-red-400">{errors.role.message}</span>}
            </div>
            <div className="flex items-baseline justify-between">
              <button className="w-full px-6 py-2 mt-4 text-white bg-blue-600 rounded-lg hover:bg-blue-900">
                Registrar
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Register;