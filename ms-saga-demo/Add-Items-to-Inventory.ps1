$post_body_as_jason = '{"id": 1,"products": [{"id": 121,"name": "prod1","unitPrice": 50,"unitsAvaialable": 1000},{"id": 122,"name": "prodd2","unitPrice": 250,"unitsAvaialable": 1000},{"id": 123,"name": "prodd3","unitPrice": 10,"unitsAvaialable": 1000},{"id": 124,"name": "prodd4","unitPrice": 500,"unitsAvaialable": 1000},{"id": 125,"name": "prodd5","unitPrice": 100,"unitsAvaialable": 1000}]}'

Invoke-RestMethod -Method POST `
                  -Body $post_body_as_jason `
                  -Uri http://localhost:3003/Inventory `
                  -ContentType application/json
