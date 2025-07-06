import json
from typing import Dict, Any


'''
Útmutató a fájl függvényeinek a használatához

Új felhasználó hozzáadása:

new_user = {
    "id": 4,  # Egyedi felhasználó azonosító
    "name": "Szilvás Szabolcs",
    "email": "szabolcs@plumworld.com"
}

Felhasználó hozzáadása a JSON fájlhoz:

add_user(new_user)

Hozzáadunk egy új kosarat egy meglévő felhasználóhoz:

new_basket = {
    "id": 104,  # Egyedi kosár azonosító
    "user_id": 2,  # Az a felhasználó, akihez a kosár tartozik
    "items": []  # Kezdetben üres kosár
}

add_basket(new_basket)

Új termék hozzáadása egy felhasználó kosarához:

user_id = 2
new_item = {
    "item_id": 205,
    "name": "Szilva",
    "brand": "Stanley",
    "price": 7.99,
    "quantity": 3
}

Termék hozzáadása a kosárhoz:

add_item_to_basket(user_id, new_item)

Hogyan használd a fájlt?

Importáld a függvényeket a filehandler.py modulból:

from filehandler import (
    add_user,
    add_basket,
    add_item_to_basket,
)

 - Hiba esetén ValuErrort kell dobni, lehetőség szerint ezt a 
   kliens oldalon is jelezni kell.

'''

# from filereader import (
#     get_user_by_id,
#     get_basket_by_user_id,
#     get_all_users,
#     get_total_price_of_basket
# )

# A JSON fájl elérési útja
JSON_FILE_PATH = "data/data.json"

def load_json() -> Dict[str, Any]:
    with open(JSON_FILE_PATH, "r", encoding="utf-8") as file:
        try:
            return json.load(file)
        except json.JSONDecodeError:
            raise ValueError("Hibás JSON formátum.")

def save_json(data: Dict[str, Any]) -> None:
    with open(JSON_FILE_PATH, "w", encoding='utf-8') as file:
        json.dump(data, file, indent=4, ensure_ascii=False)

def add_user(user: Dict[str, Any]) -> None:
    data = load_json()
    if any(existing["id"] == user["id"] for existing in data["Users"]):
        raise ValueError("A felhasználó már létezik.")
    data["Users"].append(user)
    save_json(data)

def add_basket(basket: Dict[str, Any]) -> None:
    data = load_json()
    if not any(u["id"] == basket["user_id"] for u in data["Users"]):
        raise ValueError("Felhasználó nem található.")
    if any(b["user_id"] == basket["user_id"] for b in data["Baskets"]):
        raise ValueError("A kosár már létezik ehhez a felhasználóhoz.")
    data["Baskets"].append(basket)
    save_json(data)

def add_item_to_basket(user_id: int, item: Dict[str, Any]) -> None:
    data = load_json()
    if not any(u["id"] == user_id for u in data["Users"]):
        raise ValueError("Felhasználó nem található.")
    elif not any(b["user_id"] == user_id for b in data["Baskets"]):
        raise ValueError("A felhasználónak nincsen kosara.")
    elif any(i["item_id"] == item["item_id"] and b["user_id"] == user_id for b in data["Baskets"] for i in b["items"]):
        for b in data["Baskets"]:
            if (b["user_id"] == user_id):
                for i in b["items"]:
                    if (i["item_id"] == item["item_id"]):
                        i["quantity"] += item["quantity"]
                        break
    else:
        for b in data["Baskets"]:
            if (b["user_id"] == user_id):
                b["items"].append(item)
                break
    save_json(data)