FROM node:current-slim as node
WORKDIR /app
RUN npm install -g npm@latest >/dev/null 2ENV NODE_ENV=local-development
EXPOSE 4200
EXPOSE 9229
USER node
