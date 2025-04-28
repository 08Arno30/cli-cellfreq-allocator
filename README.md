# cli-cellfreq-allocator

## About

The goal of this program is to read cell tower data from a file and allocate frequencies to the cell towers where towers that are furthest away from each other have the same frequency. It is important to note that some towers may be close to one another while also having the same tower being furthest away from each of those towers.

For example:
- Cell Tower X is close to Cell Towers Y and Z, and the furthest away from Cell Tower A.
- Cell Tower Y is close to Cell Towers X and B, and the furthest away from Cell Tower A.

In this case, Cell Tower X cannot have the same frequency as either Cell Tower Y or Cell Tower Z. The same counts for Cell Tower Y not being able to have the same frequency as Cell Tower X or Cell Tower B.

Additionally, Cell Tower X and Cell Tower A must have the same frequency. This is because Cell Tower X is the furthest away from Cell Tower A. The same counts for Cell Tower Y and Cell Tower A. This is because Cell Tower Y is the furthest away from Cell Tower A.

Since the program will first allocate a frequency to both Cell Tower X and Cell Tower A, it means that Cell Tower Y cannot have the same frequency as Cell Tower A. It will need another frequency because it cannot have the same frequency as Cell Tower X. Therefore, the furthest tower will maintain the frequency of the first tower identified as being furthest away from it.

## How it works

1. The program will calculate the distance between each cell tower and all other cell towers, and generate a JSON file containing the distances between cell towers.
2. Following this, the program reads the JSON data to determine which towers are close or far from each tower.
3. Towers that are close to one another cannot have the same frequency. Towers that are the furthest away from each other can have the same frequency.
4. If there are any cell towers that cannot be assigned a frequency, then it maintains a default frequency(user-defined as -1).
