# Graphical 
 
 [![Build status](https://ci.appveyor.com/api/projects/status/8j96lm7aucye9d87/branch/master?svg=true)](https://ci.appveyor.com/project/alvpickmans/graphical/branch/master)

**Graphical** is a personal research project where I explore different topics I am interested about like algorithm theory, geometry, data structures or computer graphics.


## History

**Graphical** was originally conceived as a purely [Dynamo](https://github.com/DynamoDS) package, where I tried to understand and implement visibility graphs in order to find the shortest path on a given layout.

I soon realized that Dynamo's geometry library was too heavy for this purpose, so I started to create custom geometry primitives (vertices, vectors, lines...) in order to improve the performance. 

The result is the detachment of the logic used for the construction and computation of graph and visibilities from the purely Dynamo implementation. This might give more flexibility in the future to be aplied in other platforms.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* [Dynamo](https://github.com/DynamoDS) team for the great platform.
* [Christian Reksten-Monsen](https://github.com/TaipanRex) for the inspiration and guidance from his [pyvisgraph](https://github.com/TaipanRex/pyvisgraph) project.


