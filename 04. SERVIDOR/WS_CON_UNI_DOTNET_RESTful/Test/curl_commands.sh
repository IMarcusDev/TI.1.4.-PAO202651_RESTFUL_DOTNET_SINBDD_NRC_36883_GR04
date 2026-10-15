# Login
curl \
  -X POST "http://localhost:5013/WS_CON_UNI_JAVA_RESTFUL/api/login" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{
    "username": "user",
    "password": "1234"
  }'

# Convert
curl \
  -X POST "http://localhost:5013/WS_CON_UNI_JAVA_RESTFUL/api/converter" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -H "Authorization: 807dfd86-9a97-47fd-a88c-04d73129273a" \
  -d '{
    "type": "Longitud",
    "value": "1234",
    "from": "cm",
    "to": "m"
  }'

# Get Catalog
curl \
  -X GET "http://localhost:5013/WS_CON_UNI_JAVA_RESTFUL/api/catalog/longitud" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -H "Authorization: 818abc6d-5673-4ce7-bf50-8676251ecf8b"