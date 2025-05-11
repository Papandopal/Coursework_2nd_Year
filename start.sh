#!/bin/bash

cloudflared tunnel --url http://localhost:5000 > cloudflared.log 2>&1 &

sleep 10

TUNNEL_URL=$(grep -oP 'https://[a-zA-Z0-9-]+\.trycloudflare\.com' cloudflared.log)

if [ -z "$TUNNEL_URL" ]; then
  echo "Ошибка: не удалось получить URL туннеля. Проверьте cloudflared.log."
  cat cloudflared.log
  exit 1
fi

echo "Ваша игра доступна по адресу: $TUNNEL_URL"

export TUNNEL_URL="$TUNNEL_URL"

dotnet Agar.io_Alpfa.dll
