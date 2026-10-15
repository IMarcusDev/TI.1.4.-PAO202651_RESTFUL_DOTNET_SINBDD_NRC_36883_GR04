# Login
curl \
  -X POST "http://localhost:5013/login" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{
    "username": "user",
    "password": "1234"
  }'

# Convert
curl \
  -X POST "http://localhost:5013/converter" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -H "Authorization: f60b74ed-39a4-463b-a0ac-110b001aa6ae" \
  -d '{
    "type": "Longitud",
    "value": "1234",
    "from": "cm",
    "to": "m"
  }'

# Get Catalog
curl \
  -X GET "http://localhost:5013/catalog/longitud" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -H "Authorization: f60b74ed-39a4-463b-a0ac-110b001aa6ae"