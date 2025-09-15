# ===== build =====
FROM node:20 AS build
WORKDIR /app
COPY ./marcador-frontend/package*.json ./
RUN npm ci
COPY ./marcador-frontend .
# build prod (ajusta si usas Angular <=15)
RUN npm run build

# ===== nginx =====
FROM nginx:alpine
# copia la app Angular
COPY --from=build /app/dist/ /usr/share/nginx/html
# nginx.conf propio (con proxy /api → http://api:5051)
COPY ./marcador-frontend/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
