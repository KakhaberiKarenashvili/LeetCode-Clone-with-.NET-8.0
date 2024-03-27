#include <iostream>

int main() {
    int num1, num2;
    
    // Input
    std::cout << "Enter the first number: ";
    std::cin >> num1;
    
    std::cout << "Enter the second number: ";
    std::cin >> num2;
    
    // Processing
    int sum = num1 + num2;
    
    // Output
    std::cout << "Sum: " << sum << std::endl;
    
    return 0;
}