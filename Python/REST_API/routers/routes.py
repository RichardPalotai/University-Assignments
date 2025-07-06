from schemas.schema import User, Basket, Item
from fastapi.responses import JSONResponse, RedirectResponse
from fastapi import FastAPI, HTTPException, Request, Response, Cookie
from fastapi import APIRouter
from data.filehandler import (
    add_user,
    add_basket,
    add_item_to_basket,
    load_json,
    save_json
)
from data.filereader import (
    get_user_by_id,
    get_basket_by_user_id,
    get_all_users,
    get_total_price_of_basket
)


'''

Útmutató a fájl használatához:

- Minden route esetén adjuk meg a response_modell értékét (típus)
- Ügyeljünk a típusok megadására
- A függvények visszatérési értéke JSONResponse() legyen
- Minden függvény tartalmazzon hibakezelést, hiba esetén dobjon egy HTTPException-t
- Az adatokat a data.json fájlba kell menteni.
- A HTTP válaszok minden esetben tartalmazzák a 
  megfelelő Státus Code-ot, pl 404 - Not found, vagy 200 - OK

'''


routers = APIRouter()

@routers.post('/adduser', response_model=User)
def adduser(user: User) -> User:
    try:
        add_user(user.model_dump())
        return JSONResponse(content=user.model_dump(), status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))

@routers.post('/addshoppingbag')
def addshoppingbag(userid: int) -> str:
    data = load_json()
    try:
        add_basket({"id": max((b["id"] for b in data["Baskets"]), default=100)+1, "user_id": userid, "items":[]})
        return JSONResponse(content={"message": "Kosár létrehozva."}, status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))

@routers.post('/additem', response_model=Basket)
def additem(userid: int, item: Item) -> Basket:
    try:
        add_item_to_basket(userid, {"item_id": item.item_id, "name": item.name, "brand": item.brand, "price": item.price, "quantity": item.quantity})
        return JSONResponse(content={"message": "Termék sikeresen hozzáadva a kosárhoz"}, status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))

@routers.put('/updateitem')
def updateitem(userid: int, itemid: int, updateItem: Item) -> Basket:
    try:
        data = load_json()
        if not any(u["id"] == userid for u in data["Users"]):
            raise ValueError("Felhasználó nem található.")
        if not any(b["user_id"] == userid for b in data["Baskets"]):
            raise ValueError("A felhasználónak nincsen kosara.")
        if any(i["item_id"] == itemid and b["user_id"] == userid for b in data["Baskets"] for i in b["items"]):
            for b in data["Baskets"]:
                if (b["user_id"] == userid):
                    for i in b["items"]:
                        if (i["item_id"] == itemid):
                            i["item_id"] = updateItem.item_id
                            i["name"] = updateItem.name
                            i["brand"] = updateItem.brand
                            i["price"] = updateItem.price
                            i["quantity"] = updateItem.quantity
                            break
        else:
            raise HTTPException(status_code=404, detail="Nincs ilyen termék")
        save_json(data)
        return JSONResponse(content={"message": "Termék sikeresen módosítva lett"}, status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))
        

@routers.delete('/deleteitem')
def deleteitem(userid: int, itemid: int) -> Basket:
    try:
        data = load_json()
        if not any(u["id"] == userid for u in data["Users"]):
            raise ValueError("Felhasználó nem található.")
        if not any(b["user_id"] == userid for b in data["Baskets"]):
            raise ValueError("A felhasználónak nincsen kosara.")
        if any(i["item_id"] == itemid and b["user_id"] == userid for b in data["Baskets"] for i in b["items"]):
            basket = get_basket_by_user_id(userid)
            for b in data["Baskets"]:
                if(b["user_id"] == userid):
                    b["items"] = [i for i in basket if(i["item_id"] != itemid)]
        else:
            raise HTTPException(status_code=404, detail="Nincs ilyen termék")
        save_json(data)
        return JSONResponse(content={"message": "Termék sikeresen törölve lett"}, status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))

@routers.get('/user')
def user(userid: int) -> User:
    try:
        return JSONResponse(content=get_user_by_id(userid), status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))

@routers.get('/users')
def users() -> list[User]:
    try:
        return JSONResponse(content=get_all_users(), status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))

@routers.get('/shoppingbag')
def shoppingbag(userid: int) -> list[Item]:
    try:
        return JSONResponse(content=get_basket_by_user_id(userid), status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))

@routers.get('/getusertotal')
def getusertotal(userid: int) -> float:
    try:
        return JSONResponse(content=get_total_price_of_basket(userid), status_code=200)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))
