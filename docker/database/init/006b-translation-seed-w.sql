-- =====================
-- Email Templates
-- =====================

-- booking_confirmation - subject
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 1, 'subject', 1, 'Booking Confirmation - {reservation_code}'),
('email_template', 1, 'subject', 2, 'Confirmación de Reserva - {reservation_code}');

-- booking_confirmation - body
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 1, 'body', 1, 'Dear {first_name} {last_name},

Thank you for your booking with Viajes Altairis.

Reservation Code: {reservation_code}
Hotel: {hotel_name}
Check-in: {check_in_date}
Check-out: {check_out_date}
Total: {total_price} {currency}

Please keep this email for your records. If you need to make any changes, please contact us quoting your reservation code.

Best regards,
Viajes Altairis'),
('email_template', 1, 'body', 2, 'Estimado/a {first_name} {last_name},

Gracias por su reserva con Viajes Altairis.

Código de Reserva: {reservation_code}
Hotel: {hotel_name}
Check-in: {check_in_date}
Check-out: {check_out_date}
Total: {total_price} {currency}

Por favor, conserve este correo para sus registros. Si necesita realizar algún cambio, contáctenos indicando su código de reserva.

Un cordial saludo,
Viajes Altairis');

-- booking_cancellation - subject
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 2, 'subject', 1, 'Booking Cancellation - {reservation_code}'),
('email_template', 2, 'subject', 2, 'Cancelación de Reserva - {reservation_code}');

-- booking_cancellation - body
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 2, 'body', 1, 'Dear {first_name} {last_name},

Your booking has been cancelled.

Reservation Code: {reservation_code}
Hotel: {hotel_name}
Cancellation Penalty: {penalty_amount} {currency}
Refund Amount: {refund_amount} {currency}

If you did not request this cancellation, please contact us immediately.

Best regards,
Viajes Altairis'),
('email_template', 2, 'body', 2, 'Estimado/a {first_name} {last_name},

Su reserva ha sido cancelada.

Código de Reserva: {reservation_code}
Hotel: {hotel_name}
Penalización: {penalty_amount} {currency}
Importe Reembolsado: {refund_amount} {currency}

Si usted no solicitó esta cancelación, contáctenos inmediatamente.

Un cordial saludo,
Viajes Altairis');

-- payment_received - subject
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 3, 'subject', 1, 'Payment Received - {reservation_code}'),
('email_template', 3, 'subject', 2, 'Pago Recibido - {reservation_code}');

-- payment_received - body
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 3, 'body', 1, 'Dear {first_name} {last_name},

We have received your payment.

Reservation Code: {reservation_code}
Amount: {amount} {currency}
Payment Method: {payment_method}
Transaction Reference: {transaction_reference}
Date: {payment_date}

Thank you for choosing Viajes Altairis.

Best regards,
Viajes Altairis'),
('email_template', 3, 'body', 2, 'Estimado/a {first_name} {last_name},

Hemos recibido su pago.

Código de Reserva: {reservation_code}
Importe: {amount} {currency}
Método de Pago: {payment_method}
Referencia: {transaction_reference}
Fecha: {payment_date}

Gracias por elegir Viajes Altairis.

Un cordial saludo,
Viajes Altairis');

-- invoice_created - subject
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 4, 'subject', 1, 'Invoice {invoice_number}'),
('email_template', 4, 'subject', 2, 'Factura {invoice_number}');

-- invoice_created - body
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 4, 'body', 1, 'Dear {first_name} {last_name},

Your invoice is now available.

Invoice Number: {invoice_number}
Period: {period_start} - {period_end}
Subtotal: {subtotal} {currency}
Tax: {tax_amount} {currency}
Total: {total_amount} {currency}

Best regards,
Viajes Altairis'),
('email_template', 4, 'body', 2, 'Estimado/a {first_name} {last_name},

Su factura está disponible.

Número de Factura: {invoice_number}
Período: {period_start} - {period_end}
Subtotal: {subtotal} {currency}
Impuestos: {tax_amount} {currency}
Total: {total_amount} {currency}

Un cordial saludo,
Viajes Altairis');

-- discount_granted - subject
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 5, 'subject', 1, 'You have received a special discount!'),
('email_template', 5, 'subject', 2, '¡Has recibido un descuento especial!');

-- discount_granted - body
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 5, 'body', 1, 'Dear {first_name} {last_name},

Great news! You have been granted a special discount of {discount_percentage}% on all future bookings.

This discount will be automatically applied to your next reservation.

Thank you for being a valued customer of Viajes Altairis.

Best regards,
Viajes Altairis'),
('email_template', 5, 'body', 2, 'Estimado/a {first_name} {last_name},

¡Buenas noticias! Se le ha concedido un descuento especial del {discount_percentage}% en todas sus futuras reservas.

Este descuento se aplicará automáticamente en su próxima reserva.

Gracias por ser un cliente valioso de Viajes Altairis.

Un cordial saludo,
Viajes Altairis');

-- subscription_welcome - subject
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 6, 'subject', 1, 'Welcome to {subscription_type}!'),
('email_template', 6, 'subject', 2, '¡Bienvenido a {subscription_type}!');

-- subscription_welcome - body
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 6, 'body', 1, 'Dear {first_name} {last_name},

Welcome to the {subscription_type} subscription!

Your subscription is now active and you will enjoy a {discount_percentage}% discount on all bookings.

Monthly fee: {price_per_month} {currency}
Start date: {start_date}

Thank you for choosing Viajes Altairis.

Best regards,
Viajes Altairis'),
('email_template', 6, 'body', 2, 'Estimado/a {first_name} {last_name},

¡Bienvenido/a a la suscripción {subscription_type}!

Su suscripción está activa y disfrutará de un {discount_percentage}% de descuento en todas sus reservas.

Cuota mensual: {price_per_month} {currency}
Fecha de inicio: {start_date}

Gracias por elegir Viajes Altairis.

Un cordial saludo,
Viajes Altairis');

-- subscription_expiring - subject
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 7, 'subject', 1, 'Your {subscription_type} subscription is expiring soon'),
('email_template', 7, 'subject', 2, 'Su suscripción {subscription_type} está por vencer');

-- subscription_expiring - body
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('email_template', 7, 'body', 1, 'Dear {first_name} {last_name},

Your {subscription_type} subscription will expire on {end_date}.

To continue enjoying your {discount_percentage}% discount on all bookings, please renew your subscription before the expiration date.

Best regards,
Viajes Altairis'),
('email_template', 7, 'body', 2, 'Estimado/a {first_name} {last_name},

Su suscripción {subscription_type} vencerá el {end_date}.

Para seguir disfrutando de su {discount_percentage}% de descuento en todas sus reservas, renueve su suscripción antes de la fecha de vencimiento.

Un cordial saludo,
Viajes Altairis');
