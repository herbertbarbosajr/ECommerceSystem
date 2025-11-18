import React from 'react';

interface CustomInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  labelText: string;
  id: string;
  handleChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

export default function CustomInput({ labelText, id, handleChange, ...props }: CustomInputProps) {
  return (
    <div className="w-full mb-4">
      <label htmlFor={id} className="block text-sm font-medium text-gray-700 mb-1">
        {labelText}
      </label>
      <input
        id={id}
        onChange={handleChange}
        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        {...props}
      />
    </div>
  );
}