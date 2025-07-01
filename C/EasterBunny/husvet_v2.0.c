#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <signal.h>     // NEW
#include <sys/types.h>  // NEW
#include <unistd.h>     // NEW
#include <sys/wait.h>   // NEW
#include <time.h>       // NEW

#define MAX_BUNNIES 50


#define MAX_BUNNIES 50

typedef struct {
    char name[20];
    char poem[100];
    int eggs;
} Bunny;

// Display menu
void display_menu(char *menu_input){
    printf("## ============================= ##\n");
    printf("||    Húsvéti locsoló verseny    ||\n");
    printf("## ============================= ##\n\n");
    printf("0 -------- Játék szabály -------- 0\n");
    printf("1 --- Új résztvevő hozzáadása --- 1\n");
    printf("2 ----- Győztes kihirdetése ----- 2\n");
    printf("3 ------ Adatok módosítása ------ 3\n");
    printf("4 ------ Résztvevő törlése ------ 4\n");
    printf("5 ------ Résztvevők adatai ------ 5\n");
    printf("6 ------- Locsoló verseny ------- 6\n");
    printf("7 ---------- Kilépés ------------ 7\n\n");
    printf("Választás -> ");
    scanf("%s", menu_input);
    if (strchr("01234567", menu_input[0]) == NULL || menu_input[1] != '\0')
    {
        menu_input[0] = ' '; // Make that input invalid
    }
}

// User Guide
void display_guide(){
    printf("## ====== Játék szabályok ====== ##\n\n");
    printf("1 --- Új résztvevő hozzáadása --- 1\n");
    printf("Add meg az új játékos nevét és a verset amivel benevezett a versenyre\n\n");
    
    printf("2 ----- Győztes kihirdetése ----- 2\n");
    printf("Megadja, hogy melyik nyuszi gyűjtötte a legtöbb tojást\n\n");

    printf("3 ------ Adatok módosítása ------ 3\n");
    printf("Add meg a nevét melyik nyuszinak akarod módosítnai a tojásainak számát majd azt hogy mennyivel (pl.: -1, 1, 2, 3, stb...)\n\n");

    printf("4 ------ Résztvevő törlése ------ 4\n");
    printf("Add meg annak a résztvevőnek a nevét, amelyiket törölni akarod a versenyből\n\n");

    printf("5 ------ Résztvevők adatai ------ 5\n");
    printf("Kiírja a verseny jelenlegi állását\n\n");

    printf("6 ------- Locsoló verseny ------- 6\n");
    printf("Elindítja a locsoló versenyt, maximum 50 nyuszi lehet regisztrálva a locsoló versenyre\n\n");

    printf("7 ---------- Kilépés ------------ 7\n");
    printf("Kilép a játékból\n\n");
    printf("## ====== Játék szabályok ====== ##\n\n");
}


// Gets and checks if there was an error, the input was too long or the input was empty
void validate_input(char *buffer, size_t size, const char *prompt){
    int isValid = 1;
    do {
        isValid = 1;
        printf("%s", prompt);
        if (fgets(buffer, size, stdin) == NULL) {
            printf("Hiba történt a bemenetnél!\n");
            isValid = 0;
        }

        // Check overflow
        else if (strchr(buffer, '\n') == NULL) {
            printf("Túl hosszú input! Tisztítom a bemenetet.\n");
            int ch;
            while ((ch = getchar()) != '\n' && ch != EOF);
            isValid = 0;
        }
        
        buffer[strcspn(buffer, "\n")] = '\0';

        if (strlen(buffer) == 0) {
            printf("Üres input! Próbáld újra.\n");
            isValid = 0;
        }

    }while (isValid != 1);
}

// Gets and checks int input if it's negative or not a number
void validate_int_input(int *buffer, const char *prompt){
    printf("%s", prompt);
    while (scanf("%d", buffer) != 1 || *buffer < 0) {
        printf("Érvénytelen bemenet! (szöveg vagy negatív szám)\n%s", prompt);
        while (getchar() != '\n'); // clear buffer
    }
    while (getchar() != '\n'); // clear buffer after scanf
}

// Check if the given name is in the txt file
int check_name(char *name, const char *filename){
    FILE *file = fopen(filename, "r");
    if(file == NULL){
        //perror("Nem sikerült megnyitni a fájlt");
        return 0;
    }

    char line[512];

    while(fgets(line, sizeof(line), file) != NULL){
        line[strcspn(line, "\n")] = '\0';

        char *data = strtok(line, ";");
        if (data != NULL && (strcasecmp(data, name) == 0)){
            fclose(file);
            return 1;
        }
    }

    fclose(file);
    return 0;
}

// Adds a new bunny to the file
void add_bunny(const char *filename){
    Bunny bunny;
    bunny.eggs = 0;
    
    // Check the valdity of the inputs
    validate_input(bunny.name, sizeof(bunny.name), "Add meg a nyuszi nevét: ");
    if(check_name(bunny.name, filename) != 0){
        printf("%s nyuszi már nevezett a versenyre!\n", bunny.name);
        return;
    }
    validate_input(bunny.poem, sizeof(bunny.poem), "Add meg a verset: ");
    validate_int_input(&bunny.eggs, "Add meg a kapott tojások számát: ");

    // Open file for appending
    FILE *file = fopen(filename, "a");
    if (file == NULL) {
        perror("Nem sikerült megnyitni a fájlt");
        exit(EXIT_FAILURE);
    }

    // Write to file (name;verse;eggcount\n)
    fprintf(file, "%s;%s;%d\n", bunny.name, bunny.poem, bunny.eggs);

    printf("%s nyuszi bekerült a versenyzők közé!\n", bunny.name);

    fclose(file);
}

// Determines the winner of the game
void winner(const char *filename){
    FILE *file = fopen(filename, "r");
    if(file == NULL){
        perror("Nem sikerült megnyitni a fájlt");
        return;
    }

    char line[512];
    char winner_bunny[20] = "";
    int eggCount = 0;
    while(fgets(line, sizeof(line), file) != NULL){
        line[strcspn(line, "\n")] = '\0';

        char *data = strtok(line, ";");
        char *name = data;
        data = strtok(NULL, ";");
        data = strtok(NULL, ";");
        if(atoi(data) > eggCount){
            eggCount = atoi(data);
            strcpy(winner_bunny, name);
        }
    }

    if (strlen(winner_bunny) > 0) {
        printf("%s nyuszi győzött %d db tojással.\n", winner_bunny, eggCount);
    } else {
        printf("Nincs nyertes nyuszi.\n");
    }

    fclose(file);
}

// Changes the egg count for a given bunny
void change_data(const char *filename){
    FILE *file = fopen(filename, "r");
    if(file == NULL){
        perror("Nem sikerült megnyitni a fájlt");
        return;
    }

    char bunny_name[20];
    do
    {
        validate_input(bunny_name, sizeof(bunny_name), "Add meg a módosítandó nyuszi nevét: ");
        if(check_name(bunny_name, filename) != 1){
            printf("Nincs %s nevű nyuszi!\n", bunny_name);
        }
    } while (check_name(bunny_name, filename) != 1);
    Bunny new_bunny;
    new_bunny.eggs = 0;
    validate_input(new_bunny.name, sizeof(new_bunny.name), "Add meg az új nevet: ");
    if(strcmp(bunny_name, new_bunny.name) != 0 && check_name(new_bunny.name, filename) != 0){
        printf("%s nyuszi már nevezett a versenyre!\n", new_bunny.name);
        return;
    }
    validate_input(new_bunny.poem, sizeof(new_bunny.poem), "Add meg az új verset: ");
    printf("%s", "Add meg hogy mennyi tojást szeretnél elveni/hozzáadni: ");
    while (scanf("%d", &new_bunny.eggs) != 1) {
        printf("Érvénytelen bemenet!\n%s", "Add meg hogy mennyi tojást szeretnél elveni/hozzáadni: ");
        while (getchar() != '\n'); // clear buffer
    }
    while (getchar() != '\n'); // clear buffer after scanf

    FILE *temp = fopen("temp.txt", "w");
    if(temp == NULL){
        perror("Nem sikerült megnyitni az ideiglenes fájlt");
        fclose(temp);
        return;
    }

    char line[512];
    while (fgets(line, sizeof(line), file) != NULL){
        line[strcspn(line, "\n")] = '\0';

        char line_copy[512];
        strcpy(line_copy, line);

        char *data = strtok(line_copy, ";");
        char *name = data;

        if(name != NULL && strcmp(name, bunny_name) == 0){
            char *datas[50];
            int dataCount = 0;
            
            data = strtok(line, ";");
            while (data != NULL && dataCount < 50){
                datas[dataCount++] = data;
                data = strtok(NULL, ";");
            }

            strcpy(datas[0], new_bunny.name);
            strcpy(datas[1], new_bunny.poem);
            
            char egg[10];
            if(atoi(datas[dataCount-1])+new_bunny.eggs < 0){
                snprintf(egg, sizeof(egg), "%d", 0);
            }
            else{
                snprintf(egg, sizeof(egg), "%d", atoi(datas[dataCount-1])+new_bunny.eggs);
            }
            datas[dataCount-1] = egg;

            for (int i = 0; i < dataCount; i++){
                fprintf(temp, "%s", datas[i]);
                if(i < dataCount-1){
                    fprintf(temp, ";");
                }
            }
            fprintf(temp, "\n");
        }
        else{
            fprintf(temp, "%s\n", line);
        }
    }

    fclose(file);
    fclose(temp);

    remove(filename);
    rename("temp.txt", filename);
}

// Removes a given bunny from the file
void remove_bunny(const char *filename){
    FILE *file = fopen(filename, "r");
    if(file == NULL){
        perror("Nem sikerült megnyitni a fájlt");
        return;
    }

    char bunny_name[20];
    do
    {
        validate_input(bunny_name, sizeof(bunny_name), "Add meg a nyuszi nevét: ");
        if(check_name(bunny_name, filename) != 1){
            printf("Nincs %s nevű nyuszi!\n", bunny_name);
        }
    } while (check_name(bunny_name, filename) != 1);

    FILE *temp = fopen("temp.txt", "w");
    if(temp == NULL){
        perror("Nem sikerült megnyitni az ideiglenes fájlt");
        fclose(temp);
        return;
    }

    char line[512];
    while (fgets(line, sizeof(line), file) != NULL){
        line[strcspn(line, "\n")] = '\0';

        char line_copy[512];
        strcpy(line_copy, line);

        char *data = strtok(line_copy, ";");
        char *name = data;

        if(name == NULL || strcmp(name, bunny_name) != 0){
            fprintf(temp, "%s\n", line);
        }
    }

    fclose(file);
    fclose(temp);

    remove(filename);
    rename("temp.txt", filename);
}

// Displays the data in the file
void display_results(const char *filename){
    FILE *file = fopen(filename, "r");
    if (file == NULL){
        perror("Nem sikerült megnyitni a fájlt");
        return;
    }

    char line[512];

    printf("--------------------------\n");
    while (fgets(line, sizeof(line), file) != NULL){
        line[strcspn(line, "\n")] = '\0';

        char *datas[50];
        int dataCount = 0;

        char *data = strtok(line, ";");
        while (data != NULL && dataCount < 50){
            datas[dataCount++] = data;
            data = strtok(NULL, ";");
        }

        if (dataCount == 3){
            printf("Nyuszi neve: %s\n", datas[0]);

            printf("Verse: %s\n", datas[1]);

            printf("Tojások száma: %s\n", datas[dataCount-1]);
            printf("--------------------------\n");
        }
        else{
            printf("Hibás sor: hányzó adatok!\n");
        }
    }

    fclose(file);
}

int load_bunnies(const char *filename, Bunny bunnies[], int *bunny_count){
    FILE *file = fopen(filename, "r");
    if (file == NULL){
        perror("Nem sikerült megnyitni a fájlt");
        return 0;
    }
    char line[512];
    while (fgets(line, sizeof(line), file) != NULL){
        line[strcspn(line, "\n")] = '\0';

        char *datas[50];
        int dataCount = 0;

        char *data = strtok(line, ";");
        while (data != NULL && dataCount < 50){
            datas[dataCount++] = data;
            data = strtok(NULL, ";");
        }

        if (dataCount == 3 && *bunny_count >= MAX_BUNNIES){
            printf("Túl sok nyuszit regisztráltak a versenyre!");
            memset(bunnies, 0, sizeof(Bunny) * MAX_BUNNIES);
            *bunny_count = 0;
            fclose(file);
            return 0;
        }
        else if (dataCount == 3 && *bunny_count < MAX_BUNNIES){
            strcpy(bunnies[*bunny_count].name, datas[0]);
            strcpy(bunnies[*bunny_count].poem, datas[1]);
            bunnies[*bunny_count].eggs = atoi(datas[2]);
            (*bunny_count)++;
        }
        else{            
            printf("Hibás sor: hányzó adatok!\n");
            *bunny_count = 0;
            fclose(file);
            return 0;
        }
    }

    fclose(file);
    return 1;
}

int save_bunnies(const char *filename, Bunny bunnies[], int *bunny_count) {
    FILE *temp = fopen("temp.txt", "w");
    if (temp == NULL) {
        perror("Nem sikerült megnyitni az ideiglenes fájlt");
        return 0;
    }

    for (int i = 0; i < *bunny_count; i++) {
        fprintf(temp, "%s;%s;%d\n", bunnies[i].name, bunnies[i].poem, bunnies[i].eggs);
    }

    fclose(temp);

    // Eredeti fájl lecserélése
    remove(filename);
    if (rename("temp.txt", filename) != 0) {
        perror("Nem sikerült átnevezni a temp fájlt");
        return 0;
    }

    return 1;
}

Bunny global_bunnies[MAX_BUNNIES];
int global_bunny_count = 0;
int bunny_index = 0;

void handler(int signum) {
    if (bunny_index < global_bunny_count) {
        printf("--- BUNNY #%d ---\n", bunny_index + 1);
        printf("%s nyuszi verse: %s\n", global_bunnies[bunny_index].name, global_bunnies[bunny_index].poem);
        bunny_index++;
    }
}

void egg_hunt(const char *filename){
    Bunny bunnies[MAX_BUNNIES];
    int bunny_count = 0;
    bunny_index = 0; // Reset bunny index for the signal handler
    if(load_bunnies(filename, bunnies, &bunny_count) != 1){
        return;
    }
    memcpy(global_bunnies, bunnies, sizeof(Bunny) * bunny_count);
    global_bunny_count = bunny_count;
    
    signal(SIGUSR1, handler);

    srand(time(NULL)); //the starting value of random number generation
    int rand_egg=(rand()%19)+1; //number between 1..20

    char msg[4];
    int pipefd[2];
    if(pipe(pipefd) == -1){
        perror("Pipe hiba");
        exit(1);
    }
    pid_t child = fork();
    if (child < 0){
        perror("Fork hiba");
        exit(1);
    }
    if (child > 0)
    {
        close(pipefd[1]); // Close write end of the pipe
        // PARENT: wait for each signal
        for (int i = 0; i < bunny_count; i++) {
            pause(); // wait for signal
            read(pipefd[0], msg, sizeof(msg)); // Read egg count from pipe
            bunnies[i].eggs += atoi(msg); // Update bunny's egg count
            global_bunnies[i].eggs = bunnies[i].eggs; // Update global bunny's egg count
        }
        close(pipefd[0]); // Close read end of the pipe
        wait(NULL); // wait for child to finish
        if (save_bunnies(filename, bunnies, &bunny_count) != 1){
            printf("Nem sikerült elmenteni a nyuszikat a fájlba!\n");
            return;
        }
        // Find the bunny with the most eggs
        int king_index = 0;
        for (int i = 1; i < bunny_count; i++) {
            if (bunnies[i].eggs > bunnies[king_index].eggs) {
                king_index = i;
            }
        }
        printf("A Húsvéti locsoló király %s nyuszi, %d tojással!\n", bunnies[king_index].name, bunnies[king_index].eggs);
    }
    else
    {
        close(pipefd[0]); // Close read end of the pipe
        // CHILD: send signals with delays
        sleep(1); // Sleep so parent process can set up signal handler
        for (int i = 0; i < bunny_count; i++) {
            rand_egg=(rand()%19)+1;
            char buffer[4]; // Enough for two digits + \n + \0
            snprintf(buffer, sizeof(buffer), "%d", rand_egg);
            write(pipefd[1], buffer, strlen(buffer)+1); // Write egg count to pipe
            kill(getppid(), SIGUSR1); // send signal to parent
            sleep(1);
            printf("%s nyuszi %d tojást kapott\n\n", bunnies[i].name, rand_egg);
            fflush(stdout); // Flush stdout to ensure the message is printed immediately
            sleep(1); // wait between bunnies so the signal won't be sent too fast
        }
        close(pipefd[1]); // Close write end of the pipe
        exit(0); // child done
    }

    // for (int i = 0; i < bunny_count; i++)
    // {
    //     printf("%s nyuszi %d tojást gyűjtött!\n", bunnies[i].name, bunnies[i].eggs);
    //     printf("Verse: %s\n", bunnies[i].poem);
    // }
}

int main(){
    char menu_input[2];
    const char *filename = "bunnies.txt";

    do
    {
        // Menu base
        display_menu(menu_input);
        while (getchar() != '\n');
        printf("\n");
        
        // Menu choice handling
        switch (menu_input[0])
        {
            // User Guide
            case '0':{
                display_guide();
                break;
            }

            // Add new bunny
            case '1':{
                add_bunny(filename);
                printf("\n");
                break;
            }
            
            // Add poem to a bunny
            case '2':{
                winner(filename);
                printf("\n");
                break;
            }

            // Modify bunny data
            case '3':{
                change_data(filename);
                printf("\n");
                break;
            }
            
            // Remove bunny
            case '4':{
                remove_bunny(filename);
                printf("\n");
                break;
            }

            // Print results
            case '5':{
                display_results(filename);
                printf("\n");
                break;
            }
            
            // Egg hunt
            case '6':{
                egg_hunt(filename);
                printf("\n");
                break;
            }

            // Exit
            case '7':{
                break;
            }

            // Exception
            default:{
                printf("Nincs ilyen menü opció\n\n");
                break;
            }
        }
    } while (menu_input[0] != '7');
}