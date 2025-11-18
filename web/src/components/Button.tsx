import React from 'react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  children: React.ReactNode;
  color?: 'primary' | 'secondary';
}

export default function Button({ children, color = 'primary', className, ...props }: ButtonProps) {
  const colorClasses = {
    primary: 'bg-blue-500 hover:bg-blue-600 text-white focus:ring-blue-500',
    secondary: 'bg-gray-500 hover:bg-gray-600 text-white focus:ring-gray-500',
  };

  const baseClasses = 'w-full px-4 py-2 rounded-md font-semibold focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50';

  return (
    <button className={`${baseClasses} ${colorClasses[color]} ${className}`} {...props}>
      {children}
    </button>
  );
}