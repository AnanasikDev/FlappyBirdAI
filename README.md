# Neural Network learning to play Flappy Bird

Algorithm takes 4 input parameters *(height, vertical speed, horizontal distance to nearest column and vertical distance to the top edge of bottom column)*, multiplies them by a certain factor ("weight"), and based on this value decides whether it should jump at the current frame or not. Values are randomized for the first generation. All next generations inherit parameters of best models from previous generation and then slightly alter them (insert mutations).

A model of 45th generation reached over 6500 score in Flappy Bird (although it is a fairly simplified version of game that is not supposed to clone the original game but just demonstrate learning process of my neural network).

This project is written completely by me in learning purposes without any guidance, specific prior knowledge/skills/experience or googling. The neural network and genetic algorithm is invented and written by me, so terms and names used by me may not be accurate or widely-used.