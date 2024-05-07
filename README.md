# LinearAlgebraStd

[![MIT License][license-shield]][license-url]

Linear Algebra Library for describing geometry and mechanical systems

<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://github.com/ja72/LinearAlgebraStd)

Outline of classes in library

 - `Vector` - An n dimensional vector
 - `Matrix` - An n×m dimensional matrix
 - `SparseVector` - An n-dimensional vector where most elements are assumed to be zero.
 - `SparseMatrix` - An n×m dimensional matrix where most elements are assumed to be zero.
 - `Vector2` - A fixed size 2 vector with componets (X,Y)
 - `Matrix2` - A fixed size 2×2 matrix with components (A11,A12,A21,A22).
 - `Vector3` - A fixed size 3 vector with componets (X,Y,Z)
 - `Matrix3` - A fixed size 3×3 matrix with components (A11,A12,A13,A21,A22,A23,A31,A32,A33).
 - `Rotor2` - A 2D quaternion for in-plane rotations
 - `Quaternion` - A 3D quaternion for spatial rotations
 - `Vector21` - A vector-scalar pair used to store 2D objects in homogeneous coordinates
 - `Matrix21` - A structured matrix to operate on `Vector21`
 - `Gauss` - Linear System solver using Guassian Elimination
 - `LinearAlgebra` - Low level utility functions that operate on native arrays.
 - `NumericalMethods` - Root finding using Fixed Point Iteration and Bisection Methods

 > NOTE: Most linear algebra objects are defined as `readonly` `struct` to maintain value semantics.

<!-- ROADMAP -->
## Roadmap

- [x] Import into GitHub
- [x] Add 2×2 Matrix and basic operations with `Vector2`
- [ ] Add 3×3 Matrix and basic operations with `Vector3`
- [x] Add Planar Projective Objects
- [x] Add 2D Robotics Solver
- [ ] Add Spatial Projective Objects
- [ ] Add 3D Robotics Solver
- [ ] Unit Testing

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[license-url]: https://github.com/ja72/LinearAlgebraStd/blob/master/LICENSE.txt
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[product-screenshot]: https://github.com/ja72/LinearAlgebraStd/blob/master/LinearAlgebraConsole/2024-05-07_16_09_50-cmd.png
