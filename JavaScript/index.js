const MainPage = document.querySelector("#MainPage")
const GamePage = document.querySelector("#GamePage")
GamePage.classList.toggle('game-page')
GamePage.classList.toggle('hidden')

const DescriptionBtn = document.querySelector("#description")
const StartBtn = document.querySelector("#StartGame")

const NameField = document.querySelector("#name")
let MapSize
let Name

const PopUp = document.querySelector("#PopUp")
const PopUpContent = document.querySelector("#PopUpContent")
const closePopUpBtn = document.querySelector("#ClosePopUp")
DescriptionBtn.addEventListener("click", ShowDescription)
function ShowDescription(){
    const title = document.createElement("h2")
    title.textContent = "Railways Játékszabályok"
    const description = document.createElement("p")
    description.textContent = "A játékban egy 5x5-ös vagy 7x7-es táblán kell elhelyezni a vasúti síneket, úgy, hogy azok minden mezőt lefedjenek ahova sín rakható (az oázis kivételével bárhova rakható sín) és egy összefüggő pályát alkossanak. A mezőkre kattintva lehet váltogatni a különböző sínek között."
    
    PopUpContent.appendChild(title)
    PopUpContent.appendChild(description)
    PopUp.style.display = "flex"
}

closePopUpBtn.addEventListener("click", close)
let IsGameOver = false
function close(){
    Array.from(PopUpContent.children).forEach(child => {
        if (child.tagName == "P" || child.tagName == "H2" || child.tagName == "OL") {
            child.remove();
        }
    });
    PopUp.style.display = "none"
    
    if (IsGameOver) {
        MainPage.classList.toggle('hidden')
        GamePage.classList.toggle('hidden')
        NameField.value = ""
        document.querySelector("#easy").checked = false
        document.querySelector("#hard").checked = false
        IsGameOver = false
    }
}

StartBtn.addEventListener("click", Start)
function Start(e) {
    document.querySelector("#easy").checked ? MapSize = 5 : document.querySelector("#hard").checked ? MapSize = 7 : MapSize = 0
    Name = NameField.value
    if (MapSize != 0 && Name != ""){
        MainPage.classList.toggle('hidden')
        GamePage.classList.toggle('hidden')
        seconds = 0
        document.querySelector("#timer").textContent = "00:00"
        SetupMenu()
        GenerateTable()
        SetupTable()
    }
}

function SetupMenu() {
    StartTimer()
    document.querySelector("#nameDisplay").textContent = NameField.value
}

let timer
let seconds = 0
function StartTimer() {
    timer = setInterval(function() {
        seconds++
        let minutes = Math.floor(seconds / 60)
        let displaySeconds = seconds % 60
        document.querySelector("#timer").textContent = `${minutes < 10 ? '0' + minutes : minutes}:${displaySeconds < 10 ? '0' + displaySeconds : displaySeconds}`
    }, 1000)
}

function StopTimer() {
    clearInterval(timer)
}

let tableDiv = document.querySelector("#TableDiv")
let table = document.createElement("table")
function GenerateTable() {
    table.innerHTML = ""
    const tbody = document.createElement("tbody")
    
    for (let i = 0; i < MapSize; i++) {
        const tr = document.createElement("tr")
        for (let j = 0; j < MapSize; j++) {
            const td = document.createElement("td")
            td.style.width = "60px"
            td.style.height = "60px"
            tr.appendChild(td)
        }
        tbody.appendChild(tr)
    }
    table.appendChild(tbody)
    tableDiv.appendChild(table)
    table.style.borderCollapse = "collapse"
    table.style.alignItems = "center"
    table.style.minWidth = `${MapSize*60}px`
    table.style.minHeight = `${MapSize*60}px`
    table.addEventListener("click", ChangeField)
}

let Map = null
function SetupTable() {
    const e1 = [
        [[0, 0, false], [2, 1, false], [0, 0, false], [0, 0, false], [3, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [1, 0, false], [3, 0, false]],
        [[1, 0, false], [0, 0, false], [2, 2, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [3, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [2, 3, false], [0, 0, false], [0, 0, false]]
    ];

    const e2 = [
        [[3, 0, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [2, 2, false], [0, 0, false], [0, 0, false], [2, 2, false]],
        [[1, 0, false], [3, 0, false], [2, 3, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [3, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]]
    ];

    const e3 = [
        [[0, 0, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [1, 0, false]],
        [[0, 0, false], [2, 2, false], [1, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [3, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false], [2, 2, false]]
    ];

    const e4 = [
        [[0, 0, false], [0, 0, false], [0, 0, false], [1, 1, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[1, 0, false], [0, 0, false], [2, 1, false], [0, 0, false], [2, 1, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [3, 0, false], [2, 3, false], [0, 0, false]]
    ];

    const e5 = [
        [[0, 0, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [2, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[1, 0, false], [0, 0, false], [0, 0, false], [2, 3, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [1, 0, false], [3, 0, false], [0, 0, false]],
        [[0, 0, false], [2, 2, false], [0, 0, false], [0, 0, false], [0, 0, false]]
    ];

    const d1 = [
        [[0, 0, false], [2, 1, false], [3, 0, false], [3, 0, false], [0, 0, false], [1, 1, false], [0, 0, false]],
        [[1, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [1, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [2, 3, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[2, 3, false], [0, 0, false], [2, 1, false], [0, 0, false], [1, 1, false], [0, 0, false], [3, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false], [0, 0, false]]
    ];

    const d2 = [
        [[0, 0, false], [0, 0, false], [3, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[1, 0, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false], [2, 2, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false], [0, 0, false], [1, 0, false]],
        [[2, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [3, 0, false], [0, 0, false], [2, 1, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [2, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [3, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]]
    ];

    const d3 = [
        [[0, 0, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [1, 0, false]],
        [[3, 0, false], [0, 0, false], [2, 3, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [3, 0, false], [2, 3, false], [0, 0, false], [1, 1, false], [0, 0, false], [0, 0, false]],
        [[1, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [2, 1, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [3, 0, false], [2, 3, false], [0, 0, false], [0, 0, false], [0, 0, false]]
    ];

    const d4 = [
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [1, 0, false], [0, 0, false], [2, 2, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [2, 3, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [1, 1, false], [0, 0, false], [3, 0, false], [0, 0, false], [1, 1, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [2, 2, false], [0, 0, false], [2, 1, false], [0, 0, false], [0, 0, false]],
        [[1, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [2, 3, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]]
    ];

    const d5 = [
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [2, 0, false], [0, 0, false]],
        [[0, 0, false], [1, 1, false], [1, 1, false], [0, 0, false], [2, 1, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [2, 0, false], [0, 0, false], [3, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [2, 2, false], [0, 0, false], [1, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]],
        [[0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false], [0, 0, false]]
    ];

    const Easy = [e1, e2, e3, e4, e5];
    const Hard = [d1, d2, d3, d4, d5];

    let rndMap = Math.floor(Math.random() * (4 - 0 + 1)) + 0;
    let ChosenMap = MapSize === 5 ? Easy[rndMap] : Hard[rndMap];
    Map = ChosenMap;

    const tileMap = {
        "0,0": { image: "pics/tiles/empty.png", rotation: 0 },
        "1,0": { image: "pics/tiles/bridge.png", rotation: 0 },
        "2,0": { image: "pics/tiles/mountain.png", rotation: 0 },
        "3,0": { image: "pics/tiles/oasis.png", rotation: 0 },
        "1,1": { image: "pics/tiles/bridge.png", rotation: 90 },
        "2,1": { image: "pics/tiles/mountain.png", rotation: 90 },
        "2,2": { image: "pics/tiles/mountain.png", rotation: 180 },
        "2,3": { image: "pics/tiles/mountain.png", rotation: 270 }
    };

    for (let i = 0; i < MapSize; i++) {
        for (let j = 0; j < MapSize; j++) {
            const key = ChosenMap[i][j].slice(0, 2).toString(); // Get only the first two elements for the key
            const tile = tileMap[key];

            if (tile) {
                table.rows[i].cells[j].style.backgroundImage = `url('${tile.image}')`;
                table.rows[i].cells[j].style.backgroundSize = "cover";
                table.rows[i].cells[j].style.transform = `rotate(${tile.rotation}deg)`;
            }
        }
    }
}


const Fields = {
    0: { image: "pics/tiles/empty.png" },
    1: { image: "pics/tiles/bridge.png" },
    2: { image: "pics/tiles/mountain.png" },
    3: { image: "pics/tiles/oasis.png" },
    4: { image: "pics/tiles/straight_rail.png" },
    5: { image: "pics/tiles/curve_rail.png" },
    6: { image: "pics/tiles/mountain_rail.png" },
    7: { image: "pics/tiles/bridge_rail.png" }
}
function ChangeField(e) {
    if (e.target.tagName === "TD") {
        const row = e.target.parentElement
        const i = Array.from(table.rows).indexOf(row)
        const j = Array.from(row.cells).indexOf(e.target)
        const tileType = Map[i][j][0]

        switch (tileType) {
            case 0:
                table.rows[i].cells[j].style.transform = "rotate(0deg)"
                Map[i][j][0] = 4
                table.rows[i].cells[j].style.backgroundImage = `url('${Fields[4].image}')`
                break;
            case 1:
                Map[i][j][0] = 7
                table.rows[i].cells[j].style.backgroundImage = `url('${Fields[7].image}')`
                break;
            case 2:
                Map[i][j][0] = 6
                table.rows[i].cells[j].style.backgroundImage = `url('${Fields[6].image}')`
                break;
            case 3:
                // Nothing
                break;
            case 4:
                if (Map[i][j][1] != 1) {
                    table.rows[i].cells[j].style.transform = "rotate(90deg)"
                    Map[i][j][1]++
                }
                else {
                    table.rows[i].cells[j].style.transform = "rotate(0deg)"
                    Map[i][j][0] = 5
                    Map[i][j][1] = 0
                    table.rows[i].cells[j].style.backgroundImage = `url('${Fields[5].image}')`
                }
                break;
            case 5:
                if (Map[i][j][1] != 3) {
                    Map[i][j][1]++
                    table.rows[i].cells[j].style.transform = `rotate(${Map[i][j][1]*90}deg)`
                }
                else {
                    table.rows[i].cells[j].style.transform = "rotate(0deg)"
                    Map[i][j][0] = 0
                    Map[i][j][1] = 0
                    table.rows[i].cells[j].style.backgroundImage = `url('${Fields[0].image}')`
                }
                break;
            case 6:
                Map[i][j][0] = 2
                table.rows[i].cells[j].style.backgroundImage = `url('${Fields[2].image}')`
                break;
            case 7:
                Map[i][j][0] = 1
                table.rows[i].cells[j].style.backgroundImage = `url('${Fields[1].image}')`
                break;

            default:
                break;
        }

        console.log(`(${Map[i][j][0]}, ${Map[i][j][1]})`)
        if (CheckGameOver()){
            IsGameOver = true
            StopTimer()
            ShowResults()
        }
        const thirdElementsMatrix = Map.map(row => row.map(cell => cell[2]));
        console.log(thirdElementsMatrix);;
    }
}

function CheckGameOver() {
    for (let i = 0; i < MapSize; i++) {
        for (let j = 0; j < MapSize; j++) {
            if (IsStraightRail(i, j)){
                if (IsRotated(i, j)){
                    CheckLeft(i, j) && CheckRight(i, j) ? Map[i][j][2] = true : Map[i][j][2] = false
                }
                else{
                    CheckUp(i, j) && CheckDown(i, j) ? Map[i][j][2] = true : Map[i][j][2] = false
                }
            }
            else if (IsCurvedRail(i, j)){
                switch (Map[i][j][1]) {
                    case 0:
                        CheckDown(i, j) && CheckRight(i, j) ? Map[i][j][2] = true : Map[i][j][2] = false
                        break;
                    case 1:
                        CheckLeft(i, j) && CheckDown(i, j) ? Map[i][j][2] = true : Map[i][j][2] = false
                        break;
                    case 2:
                        CheckLeft(i, j) && CheckUp(i, j) ? Map[i][j][2] = true : Map[i][j][2] = false
                        break;
                    case 3:
                        CheckUp(i, j) && CheckRight(i, j) ? Map[i][j][2] = true : Map[i][j][2] = false
                        break;
                
                    default:
                        break;
                }
            }
            else if (IsOasis(i, j)){
                Map[i][j][2] = true
            }
        }
    }

    return Map.every(row => row.every(cell => cell[2] === true))
}

function IsOasis(i, j){
    return Map[i][j][0] == 3
}

function IsStraightRail(i, j){
    return Map[i][j][0] == 4 || Map[i][j][0] == 7
}

function IsCurvedRail(i, j){
    return Map[i][j][0] == 5 || Map[i][j][0] == 6
}

function IsRotated(i, j){
    return Map[i][j][1] != 0
}

function CheckUp(i, j){
    if (InBound(i-1, j)){
        if (Map[i-1][j][1] == 0 || (Map[i-1][j][1] == 1 && Map[i-1][j][0] != 4 && Map[i-1][j][0] != 7)){
            return true
        }
        else{
            return false
        }
    }
    else{
        return false
    }
}

function CheckDown(i, j){
    if (InBound(i+1, j)){
        if ((Map[i+1][j][1] == 0 && Map[i+1][j][0] != 5 && Map[i+1][j][0] != 6 ) || Map[i+1][j][1] == 2 || Map[i+1][j][1] == 3){
            return true
        }
        else{
            return false
        }
    }
    else{
        return false
    }
}

function CheckLeft(i, j){
    if (InBound(i, j-1)){
        if (Map[i][j-1][1] == 3 || (Map[i][j-1][1] == 1 && Map[i][j-1][0] != 5 && Map[i][j-1][0] != 6) || (Map[i][j-1][1] == 0 && Map[i][j-1][0] != 4 && Map[i][j-1][0] != 7)){
            return true
        }
        else{
            return false
        }
    }
    else{
        return false
    }
}

function CheckRight(i, j){
    if(InBound(i, j+1)){
        if(Map[i][j+1][1] == 2 || Map[i][j+1][1] == 1){
            return true
        }
        else{
            return false
        }
    }
    else{
        return false
    }
}

function InBound(i, j){
    return 0 <= i && i < MapSize && 0 <= j && j < MapSize
}

let TierListFive = []
let TierListSeven = []
function ShowResults(){
    const title = document.createElement("h2")
    title.textContent = "Eredmények"
    const description = document.createElement("p")
    const minutes = Math.floor(seconds / 60)
    const displaySeconds = seconds % 60
    minutes == 0 ? description.textContent = `${displaySeconds} másodperc alatt sikerült befejezned a pályát!` : description.textContent = `${minutes} perc és ${displaySeconds} másodperc alatt sikerült befejezned a pályát!`
    PopUpContent.appendChild(title)
    PopUpContent.appendChild(description)
    
    if (MapSize == 5){
        if (TierListFive.some(item => item[0] === Name)){
            const index = TierListFive.findIndex(item => item[0] === Name)
            if (TierListFive[index][3] > seconds){
                TierListFive[index][1] = minutes
                TierListFive[index][2] = displaySeconds
                TierListFive[index][3] = seconds
            }
        }
        else{
            TierListFive.push([Name, minutes, displaySeconds, seconds])
        }
        TierListFive.sort((a, b) => a[3] - b[3]);
    
    }
    else if (MapSize == 7){
        if (TierListSeven.some(item => item[0] === Name)){
            const index = TierListSeven.findIndex(item => item[0] === Name)
            if (TierListSeven[index][3] > seconds){
                TierListSeven[index][1] = minutes
                TierListSeven[index][2] = displaySeconds
                TierListSeven[index][3] = seconds
            }
        }
        else{
            TierListSeven.push([Name, minutes, displaySeconds, seconds])
        }
        TierListSeven.sort((a, b) => a[3] - b[3]);
        
    }

    if (TierListFive.length > 0){
        const tierListTitleFive = document.createElement("h2")
        tierListTitleFive.textContent = "5x5 Top lista"
        const tierListFive = document.createElement("ol")
        for (let i = 0; i < TierListFive.length; i++){
            const player = document.createElement("li")
            player.textContent = `${TierListFive[i][0].toUpperCase()}, ${TierListFive[i][1] == 0 ? `${TierListFive[i][2]}mp` : `${TierListFive[i][1]}p ${TierListFive[i][2]}mp`}`
            tierListFive.appendChild(player)
        }
        PopUpContent.appendChild(tierListTitleFive)
        PopUpContent.appendChild(tierListFive)
    }
    if (TierListSeven.length > 0){
        const tierListTitleSeven = document.createElement("h2")
        tierListTitleSeven.textContent = "7x7 Top lista"
        const tierListSeven = document.createElement("ol")
        for (let i = 0; i < TierListSeven.length; i++){
            const player = document.createElement("li")
            player.textContent = `${TierListSeven[i][0].toUpperCase()}, ${TierListSeven[i][1] == 0 ? `${TierListSeven[i][2]}mp` : `${TierListSeven[i][1]}p ${TierListSeven[i][2]}mp`}`
            tierListSeven.appendChild(player)
        }
        PopUpContent.appendChild(tierListTitleSeven)
        PopUpContent.appendChild(tierListSeven)
    }


    PopUp.style.display = "flex"
}