var cartItems = [];
var vatRate = 0; // Assuming default VAT rate is 15%
var discount = 0; // Assuming default discount is 0%

$(document).ready(function () {
    // Load cart items from localStorage if available
    var cartItems = JSON.parse(localStorage.getItem('cartItems')) || [];
    renderCart(cartItems);
});

function addCustomerInfo(event) {
    event.preventDefault(); // Prevent form submission
    var customerName = $('#customerName').val();
    var phoneNumber = $('#phoneNumber').val();

    // Create customer object
    var customer = {
        name: customerName,
        phoneNumber: phoneNumber
    };

    // Save customer info to localStorage
    localStorage.setItem('customer', JSON.stringify(customer));

    renderCart(); // Render cart with customer info
}

function addToCart(itemId) {
    var quantity = parseInt($('#Count_' + itemId).val());
    var itemName = $('#Name_' + itemId).text();
    var itemPrice = parseFloat($('#Price_' + itemId).text());

    var subtotal = itemPrice * quantity;

    // Create cart item object
    var cartItem = {
        itemId: itemId,
        itemName: itemName,
        quantity: quantity,
        subtotal: subtotal
    };

    // Add cart item to local storage
    var cartItems = JSON.parse(localStorage.getItem('cartItems')) || [];
    cartItems.push(cartItem);
    localStorage.setItem('cartItems', JSON.stringify(cartItems));

    renderCart(); // Render cart with updated cart items
}

function renderCart() {
    var customerInfo = JSON.parse(localStorage.getItem('customer')); // Get customer info from localStorage
    var cartItems = JSON.parse(localStorage.getItem('cartItems')) || [];

    // Render customer information
    var customerInfoHtml = '<div class="customer-info">\
                                        <span class="text-info">Customer Name:</span> ' + customerInfo.name + '<br>\
                                        <span class="text-info">Phone Number:</span> ' + customerInfo.phoneNumber + '<br><br>\
                                    </div>';

    $('#cart').html(customerInfoHtml);

    // Render cart items
    var totalPrice = 0;
    cartItems.forEach(function (item) {
        var cartItemHtml = '<div class="cart-item">';
        cartItemHtml += '<table class="table">';
        cartItemHtml += '<thead><tr class="table-info"><th>Item:</th><th>Quantity:</th><th>Price:</th></tr></thead>';
        cartItemHtml += '<tbody>';
        cartItemHtml += '<tr class="table-success"><td>' + item.itemName + '</td><td>' + item.quantity + '</td><td class="subtotal">' + item.subtotal.toFixed(2) + '</td></tr>';
        cartItemHtml += '</tbody>';
        cartItemHtml += '</table>';
        cartItemHtml += '</div>';
        $('#cart').append(cartItemHtml);
        totalPrice += item.subtotal;
    });

    // Update total price
    var totalPriceWithVAT = calculateTotalPrice(totalPrice);
    $('#totalPrice').text('Total Price (including VAT): ' + totalPriceWithVAT.toFixed(2));
}


function applyVatAndDiscount() {
    // Get VAT rate and discount values
    vatRate = parseFloat($('#vatRateInput').val()) / 100;
    discount = parseFloat($('#discountInput').val()) / 100;

    // Get cart items from local storage
    var cartItems = JSON.parse(localStorage.getItem('cartItems')) || [];

    // Update each item with VAT and discount
    var totalPrice = 0;
    cartItems.forEach(function (item) {
        var subtotal = item.subtotal;
        var discountedSubtotal = subtotal * (1 - discount);
        var totalWithVat = discountedSubtotal + (discountedSubtotal * vatRate);
        item.subtotal = totalWithVat;
        totalPrice += totalWithVat;
    });

    localStorage.setItem('cartItems', JSON.stringify(cartItems));
    renderCart(cartItems);

    // Update total price display
    var totalPriceWithVAT = calculateTotalPrice(totalPrice);
    $('#totalPrice').text('Total Price (including VAT): ' + totalPriceWithVAT.toFixed(2));
}

function calculateTotalPrice(totalPrice) {
    return totalPrice;
}


function clearLocalStorage() {
    cartItems = [];
    localStorage.removeItem('cartItems');
    localStorage.removeItem('customer');
}

function clearCart() {
    $('#cart').empty(); // Clear cart items from the HTML
    totalPrice = 0; // Reset total price
    $('#totalPrice').text('Total Price: 0.00'); // Update total price display
    clearLocalStorage(); // Clear cart items from localStorage
}
function finalizeBill() {
    var cartItems = JSON.parse(localStorage.getItem('cartItems')) || [];
    cartItems.forEach(function (item) {
        $.ajax({
            url: '/Cart/UpdateItemQuantity',
            type: 'POST',
            data: { itemId: item.itemId, quantity: item.quantity },
            success: function (response) {
                console.log('Item quantity updated successfully:', response);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log('Error updating item quantity:', errorThrown);
            }
        });
    });

}

function generatePDF() {
    var cartItems = JSON.parse(localStorage.getItem('cartItems')) || [];
    var customerInfo = JSON.parse(localStorage.getItem('customer'));

    var doc = new jsPDF();

    // Add logo
    var logo = new Image();
    logo.src = 'images/supershop.png'; 
    var logoWidth = 50;
    var logoHeight;

    logo.onload = function () {
        logoHeight = (logo.height * logoWidth) / logo.width; // Maintain aspect ratio
        var startX = (doc.internal.pageSize.width - logoWidth) / 2;
        doc.addImage(logo, 'PNG', startX, 10, logoWidth, logoHeight); // Adjust the position and size as needed

        // Add customer information
        doc.setTextColor(0, 0, 0); // Black color
        doc.setFontSize(12);
        doc.text("Customer Name: " + customerInfo.name, 10, logoHeight + 20);
        doc.text("Phone Number: " + customerInfo.phoneNumber, 10, logoHeight + 30);

        // Add table header
        doc.setFillColor(135, 206, 235); // Light blue color
        doc.rect(10, logoHeight + 40, 190, 10, 'F'); // Table header background
        doc.setTextColor(0, 0, 0); // Black color for text
        doc.text("Item", 15, logoHeight + 45);
        doc.text("Quantity", 70, logoHeight + 45);
        doc.text("Price", 120, logoHeight + 45);

        // Add cart items
        var startY = logoHeight + 55;
        cartItems.forEach(function (item) {
            doc.text(item.itemName, 15, startY);
            doc.text(item.quantity.toString(), 70, startY);
            doc.text(item.subtotal.toFixed(2), 120, startY);
            startY += 10;
        });

        // Calculate total price
        var totalPrice = cartItems.reduce((acc, cur) => acc + cur.subtotal, 0);

        // Add total price
        doc.text("Total Price (including VAT): " + calculateTotalPrice(totalPrice).toFixed(2), 10, startY + 10);

        // Save the PDF
        doc.save('bill.pdf');
    };
}