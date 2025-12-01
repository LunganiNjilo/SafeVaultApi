FROM node:18-alpine

WORKDIR /app

# Install the last version that still supports --routes
RUN npm install -g json-server@0.17.4

COPY db.json .
COPY routes.json .

EXPOSE 3000

CMD ["json-server", "db.json", "--routes", "routes.json", "--host", "0.0.0.0"]
