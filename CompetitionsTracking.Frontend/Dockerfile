FROM node:22-alpine AS build
WORKDIR /app

ARG VITE_API_BASE_URL=http://localhost:8080/api
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

COPY CompetitionsTracking.Frontend/package*.json ./
RUN npm ci

COPY CompetitionsTracking.Frontend/ ./
RUN npm run build

FROM nginx:1.27-alpine
COPY CompetitionsTracking.Frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
