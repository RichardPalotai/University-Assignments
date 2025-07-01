#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "matrix.h"

#define MAX_DIM 20

// Display menu
void display_menu(char *menu_input){
    printf("## ======================= ##\n");
    printf("|| Spiral Matrix Generator ||\n");
    printf("## ======================= ##\n\n");
    printf("0 ------ User Guide ------- 0\n");
    printf("1 --- Generate a matrix --- 1\n");
    printf("2 ----- Save a matrix ----- 2\n");
    printf("3 ----- Load a matrix ----- 3\n");
    printf("4 -- Print current matrix - 4\n");
    printf("5 --------- Exit ---------- 5\n\n");
    printf("Choice -> ");
    scanf("%s", menu_input);
    if (strchr("012345", menu_input[0]) == NULL || menu_input[1] != '\0')
    {
        menu_input[0] = 'x'; // Make that input invalid
    }
}

// User Guide
void display_guide(){
    printf("## ===== User Guide ====== ##\n\n");
    printf("1 ---- Generate matrix ---- 1\n");
    printf("You need to give the three atributes of a spiral matrix in order to generate it:\n");
    printf("1. Matrix dimension   : This must be a natural number between 1 and 20 (Including 1 and 20)\n");
    printf("2. Start direction    : Choose one diretion l(eft), r(ight), u(p) or d(down) and enter it's initial in lowercase\n");
    printf("3. Rotation direction : Choose a rotation type cw (clockwise) or ccw (counterclockwise) enter it in lowercase\n\n");
    printf("2 ----- Save a matrix ----- 2\n");
    printf("This menu saves the matrix into a file name (e.g.: spiral5jcw.txt, spiral7fccw.txt).\n");
    printf("It bases the filename on the atributes of the spiral matrix.\n\n");
    printf("3 ----- Load a matrix ----- 3\n");
    printf("This menu loads a matrix from a file and makes it the current matrix.\n");
    printf("Adequate input: '<filename>.txt' e.g: 'matrix.txt' or 'spiral5rcw.txt'\n");
    printf("If the file doesn't exist the program displays an error prompt.\n");
    printf("If you want to head back to the menu just enter 'quit'.\n\n");
    printf("4 -- Print current matrix - 4\n");
    printf("Displays the current matrix.\n\n");
    printf("5 --------- Exit ---------- 5\n");
    printf("Ends the program.\n\n");
    printf("## ===== User Guide ====== ##\n\n");
}

// Menu input handling
void matrix_atributes(int *dimension, char *start_direction, char *rotation_direction){
    int isValidNum;
    do {
        printf("Matrix dimension (1 - 20): ");
        isValidNum = scanf("%d", &(*dimension)); // Checks if input is a number
        while (getchar() != '\n'); // Clear input buffer
    } while (isValidNum != 1 || *dimension < 1 || *dimension > 20);

    do {
        printf("Start direction (l, r, u, d): ");
        scanf("%2s", start_direction); // Limit input to 2 characters + null terminator
        while (getchar() != '\n'); // Clear input buffer
    } while (strchr("lurd", start_direction[0]) == NULL || strlen(start_direction) != 1);

    do {
        printf("Rotation direction (cw, ccw): ");
        scanf("%3s", rotation_direction); // Limit input to 3 characters + null terminator
        while (getchar() != '\n'); // Clear input buffer
    } while (strcmp(rotation_direction, "cw") != 0 && strcmp(rotation_direction, "ccw") != 0);
}

// Save a matrix to file
int save_matrix(int dimension, char *start_direction, char *rotation_direction, int **current_matrix){
    if (current_matrix == NULL)
    {
        printf("There is no current matrix, generate or load one first\n");
        return 0;
    }
    char dim[3];
    char filename[30] = "spiral";
    sprintf(dim, "%d", dimension);
    strcat(filename, dim);
    strcat(filename, &start_direction[0]);
    strcat(filename, rotation_direction);
    strcat(filename, ".txt");
    //printf("%s\n", filename);

    FILE *file = fopen(filename, "w");
    if (file == NULL) {
        printf("Failed to open file for writing.\n");
        return 0;
    }

    for (int i = 0; i < dimension; ++i)
    {
        for (int j = 0; j < dimension; ++j)
        {
            fprintf(file, "%d", current_matrix[i][j]);  // Write the integer as a string
            if (j < dimension - 1)
            {
                fputc(' ', file);  // Write a space after each number except the last in a row
            }
            
        }
        if (i < dimension - 1)
        {
            fputc('\n', file);  // Write a newline after each row except the last
        }
    }
    fclose(file);
    return 1;
}

// Load a matrix from file
int load_matrix(int ***current_matrix, int *matrixExists, int *dimension){
    char *filename = NULL;
    int bufferSize = 1; // Initial buffer size
    int index = 0;
    char ch;
    FILE *file = NULL;

    filename = (char *)malloc(bufferSize * sizeof(char)); // Allocate initial memory
    if (filename == NULL) {
        printf("Memory allocation failed\n");
        return 0;
    }

    do
    {
        bufferSize = 1;
        index = 0;
        printf("Filename (e.g.: matrix.txt): ");
        while ((ch = getchar()) != '\n') {
            filename[index++] = ch;
            // If index reaches bufferSize, reallocate more memory
            if (index >= bufferSize) {
                bufferSize *= 2; // Double the buffer size
                char *temp = (char *)realloc(filename, bufferSize * sizeof(char));
                if (temp == NULL) {
                    printf("Memory reallocation failed\n");
                    free(filename);
                    return 0;
                }
                filename = temp;
            }
        }
        filename[index] = '\0'; // Null-terminate the string

        if (strcmp(filename, "quit") == 0)
        {
            free(filename);
            return 0;
        }

        file = fopen(filename, "r");
        if (file == NULL)
        {
            printf("There is no such file\n");
        }
    } while (file == NULL);

    char length_m[MAX_DIM];
    *dimension = 1; // Initialize to 1 to account for the off-by-one issue

    if (fgets(length_m, MAX_DIM, file) != NULL) { // Check that fgets() was successful
        for (int i = 0; i < strlen(length_m); i++) {
            if (length_m[i] == ' ') {
                *dimension += 1;
            }
        }
    } else {
        // Handle error or EOF
        fclose(file);
        free(filename);
        return 0;
    }

    fseek(file, 0, SEEK_SET);

    //printf("%d\n", *dimension);
    
    *matrixExists = allocate_matrix(*matrixExists, current_matrix, *dimension); // Alocate memory if there's no current matrix
    if (*matrixExists == 0)
    {
        printf("Memory allocation failed\n");
        fclose(file);
        free(filename);
        return 0;
    }
    
    // Read the file and store the numbers in the matrix
    for(int i = 0; i < *dimension; i++) {
        for(int j = 0; j < *dimension; j++) {
            // Read each number using fscanf
            if (fscanf(file, "%d", &(*current_matrix)[i][j]) != 1) {
                printf("Error reading matrix\n");
                fclose(file);
                free(filename);
                return 0;
            }
        }
    }

    fclose(file);
    free(filename);
    return 1;
}

int main(){
    char menu_input[2];
    int dimension = 0;
    char start_direction[3];
    char rotation_direction[4];
    int matrixExists = 0;
    int spiralExists = 0;
    int **current_matrix = NULL;

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
            
            // Generate a matrix
            case '1':{
                matrix_atributes(&dimension, start_direction, rotation_direction);
                printf("\n");
                matrixExists = allocate_matrix(matrixExists, &current_matrix, dimension);
                spiralExists = make_spiral(&current_matrix, start_direction, rotation_direction, dimension);

                if (matrixExists != 1 || spiralExists != 1)
                {
                    printf("Matrix generation failed\n");
                }
                break;
            }

            // Save a matrix
            case '2':{
                if(save_matrix(dimension, start_direction, rotation_direction, current_matrix)){
                    printf("Matrix saved successfully\n");
                }
                printf("\n");
                break;
            }

            // Load a matrix
            case '3':{
                if (load_matrix(&current_matrix, &matrixExists, &dimension) != 1)
                {
                    printf("Loading the matrix was aborted\n");
                }
                else{
                    printf("Matrix was loaded successfully\n");
                }
                printf("\n");
                break;
            }

            // Print current matrix
            case '4':{
                display_matrix(current_matrix, dimension);
                printf("\n");
                break;
            }

            // Exit
            case '5':{
                
                break;
            }

            // Exception
            default:{
                printf("There is no menu for this input\n\n");
                break;
            }
        }
    } while (menu_input[0] != '5');

    // Free the current_matrix's memory when program ends
    if (current_matrix != NULL)
    {
        for (int i = 0; i < dimension; ++i)
        {
            free(current_matrix[i]);
        }
        free(current_matrix);        
    }
}