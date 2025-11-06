# Use uma imagem oficial do Node.js como base
FROM node:18-alpine

# Defina o diretório de trabalho no contêiner
WORKDIR /app

# Copie o package.json e o package-lock.json (ou yarn.lock)
COPY package*.json ./

# Instale as dependências do projeto
RUN npm install

# Copie o restante do código da aplicação para o diretório de trabalho
COPY . .

# Exponha a porta em que o Vite vai rodar dentro do contêiner
EXPOSE 3000

# Comando para iniciar a aplicação em modo de desenvolvimento
# O '--host' é necessário para expor o servidor para fora do contêiner
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "3000"]