-- =============================================================================
-- Web Translation Seeds — PDF invoice labels
-- language_id: 1=en, 2=es
-- Keys prefixed with 'pdf.invoice.' for PDF generation.
-- Editable via admin Web Translations UI under the 'pdf' group.
-- =============================================================================

INSERT INTO web_translation (translation_key, language_id, value) VALUES

-- English (language_id = 1)
('pdf.invoice.invoice', 1, 'Invoice'),
('pdf.invoice.issued', 1, 'Issued'),
('pdf.invoice.status', 1, 'Status'),
('pdf.invoice.paid', 1, 'Paid'),
('pdf.invoice.bill_to', 1, 'Bill to'),
('pdf.invoice.tax_id', 1, 'Tax ID'),
('pdf.invoice.hotel', 1, 'Hotel'),
('pdf.invoice.room', 1, 'Room'),
('pdf.invoice.board', 1, 'Board'),
('pdf.invoice.check_in', 1, 'Check-in'),
('pdf.invoice.check_out', 1, 'Check-out'),
('pdf.invoice.guests', 1, 'Guests'),
('pdf.invoice.total', 1, 'Total'),
('pdf.invoice.subtotal', 1, 'Subtotal'),
('pdf.invoice.discount', 1, 'Discount'),
('pdf.invoice.tax', 1, 'Tax'),
('pdf.invoice.exchange_rate', 1, 'Exchange rate'),
('pdf.invoice.footer', 1, 'Thank you for your business.'),

-- Spanish (language_id = 2)
('pdf.invoice.invoice', 2, 'Factura'),
('pdf.invoice.issued', 2, 'Emitida'),
('pdf.invoice.status', 2, 'Estado'),
('pdf.invoice.paid', 2, 'Pagada'),
('pdf.invoice.bill_to', 2, 'Facturar a'),
('pdf.invoice.tax_id', 2, 'NIF/CIF'),
('pdf.invoice.hotel', 2, 'Hotel'),
('pdf.invoice.room', 2, 'Habitación'),
('pdf.invoice.board', 2, 'Régimen'),
('pdf.invoice.check_in', 2, 'Entrada'),
('pdf.invoice.check_out', 2, 'Salida'),
('pdf.invoice.guests', 2, 'Huéspedes'),
('pdf.invoice.total', 2, 'Total'),
('pdf.invoice.subtotal', 2, 'Subtotal'),
('pdf.invoice.discount', 2, 'Descuento'),
('pdf.invoice.tax', 2, 'Impuestos'),
('pdf.invoice.exchange_rate', 2, 'Tipo de cambio'),
('pdf.invoice.footer', 2, 'Gracias por su confianza.');
